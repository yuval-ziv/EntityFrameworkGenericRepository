#!/bin/bash

pause() {
  read -p "Press key to continue . . . " -n1 -s
}

print_job_fail() {
  job_name=$1
  job_output=$2
  job_status=$3
  
  if ((job_status != 0)); then
    echo "$job_name" failed with status "$BuildStatus".
    echo Build output:
    echo "$job_output"
    pause
    exit "$job_status"
  fi
  
  echo "$job_name" succeeded.
}

#Arguments check

if [[ -z "$1" ]] || [[ -z "$2" ]] || [[ -z "$3" ]] || [[ -z "$4" ]] || [[ -z "$5" ]]
then
  echo "ERROR: You must specify either a get, major, minor, patch or none argument, a project file, a nuget api key, path to bin directory and a build configuration (optional: release notes surround with \"\")."
  echo "Usage: $0 (get|major|minor|patch|none) MyProject.csproj nugetApiKey \\path\\to\\bin BuildConfiguration (\"Release Notes\")"
  pause
  exit 1
fi


#project file (csproj) exists.

if [[ ! -f $2 ]]
then
  echo "ERROR: The project file '$2' does not exist."
  pause
  exit 2
fi


#project file (csproj) is writeable.

if [[ ! -w $2 ]]
then
  echo "ERROR: The project file '$2' either does not exist, or is not writable."
  pause
  exit 3
fi


#bin directory exists.

if [[ ! -d $4 ]]
then
  echo "ERROR: The path to bin directory '$4' either does not exist, or is not writable."
  pause
  exit 4
fi


#Get old version

Version=$(sed -n 's/.*<Version>\(.*\)<\/Version>.*/\1/p' $2)

if [ -z "$Version" ]
then
  echo "ERROR: Could not find a <Version/> tag in the project file '$2'."
  echo "Please add one in between the <Project><PropertyGroup> tags and try again."
  pause
  exit 5
fi

echo Old Version: "$Version"


#Build

BuildOutput=$(dotnet build --configuration $5)
BuildStatus=$?

print_job_fail "Build" "$BuildOutput" "$BuildStatus"


#Test

TestOutput=$(dotnet test --configuration $5)
TestStatus=$?

print_job_fail "Test" "$TestOutput" "$TestStatus"


#Change and set version

VersionParts=(${Version//./ })
case "$1" in
  get) echo $Version; exit 0 ;;
  major) ((VersionParts[0]++)) ;;
  minor) ((VersionParts[1]++)) ;;
  patch) ((VersionParts[2]++)) ;;
  none) (()) ;;
  *)
    echo "ERROR: Invalid SemVer position name supplied, '$1' was not understood."
    echo "Usage: $0 (-get|major|minor|patch|none) $2 $3 $4 $5"
    pause
    exit 6
esac

NewVersion="${VersionParts[0]}.${VersionParts[1]}.${VersionParts[2]}"
sed -i -e "s/<Version>$Version<\/Version>/<Version>$NewVersion<\/Version>/g" $2
echo New Version: $NewVersion


#Add release notes

ReleaseNotes=$6

if [[ -z "$ReleaseNotes" ]] 
then
  echo Enter release notes:
  read -r ReleaseNotes
fi

sed -i -e "s/<PackageReleaseNotes>\(.*\)<\/PackageReleaseNotes>/<PackageReleaseNotes>$ReleaseNotes<\/PackageReleaseNotes>/g" $2


#Pack

PackOutput=$(dotnet pack --configuration Release)
PackStatus=$?

print_job_fail "Pack" "$PackOutput" "$PackStatus"


#Push

PackageId=$(sed -n 's/.*<PackageId>\(.*\)<\/PackageId>.*/\1/p' $2)
PackageFile=$4/$5/$PackageId.$NewVersion.nupkg

PushOutput=$(dotnet nuget push $PackageFile --api-key $3 --source https://api.nuget.org/v3/index.json)
PushStatus=$?

print_job_fail "Push" "$PushOutput" "$PushStatus"


#Delete package

rm $PackageFile

echo Package file deletion succeeded.

pause