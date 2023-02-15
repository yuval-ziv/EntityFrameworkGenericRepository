#!/bin/bash


#Arguments check

if [[ -z "$1" ]] || [[ -z "$2" ]] || [[ -z "$3" ]] || [[ -z "$4" ]] || [[ -z "$5" ]]
then
  echo "ERROR: You must specify either a get, major, minor, patch or none argument, a project file, a nuget api key, path to bin directory and a build configuration (optional: release notes surround with \"\")."
  echo "Usage: $0 (get|major|minor|patch|none) MyProject.csproj nugetApiKey \\path\\to\\bin BuildConfiguration (\"Release Notes\")"
  exit 1
fi


#project file (csproj) exists.

if [[ ! -f $2 ]]
then
  echo "ERROR: The project file '$2' does not exist."
  read 
  exit 2
fi


#project file (csproj) is writeable.

if [[ ! -w $2 ]]
then
  echo "ERROR: The project file '$2' either does not exist, or is not writable."
  exit 3
fi


#bin directory exists.

if [[ ! -d $4 ]]
then
  echo "ERROR: The path to bin directory '$4' either does not exist, or is not writable."
  exit 4
fi


#Get old version

Version=$(sed -n 's/.*<Version>\(.*\)<\/Version>.*/\1/p' $2)

if [ -z "$Version" ]
then
  echo "ERROR: Could not find a <Version/> tag in the project file '$2'."
  echo "Please add one in between the <Project><PropertyGroup> tags and try again."
  exit 5
fi

echo Old Version: $Version


#Build

BuildOutput=$(dotnet build --configuration $5)
BuildStatus=$?

if ((BuildStatus != 0)); then
  echo Build failed with status $BuildStatus.
  echo Build Output:
  echo $BuildOutput
  exit $BuildStatus
fi

echo Build succeeded.


#Test

TestOutput=$(dotnet test --configuration $5)
TestStatus=$?

if ((TestStatus != 0)); then
  echo Test failed with status $TestStatus.
  echo Test Output:
  echo $TestOutput
  exit $TestStatus
fi

echo Test succeeded.


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

if ((PackStatus != 0)); then
  echo Pack failed with status $PackStatus
  echo Pack Output:
  echo $PackOutput
  exit $PackStatus
fi

echo Pack succeeded.


#Push

PackageId=$(sed -n 's/.*<PackageId>\(.*\)<\/PackageId>.*/\1/p' $2)
PackageFile=$4/$5/$PackageId.$NewVersion.nupkg

PushOutput=$(dotnet nuget push $PackageFile --api-key $3 --source https://api.nuget.org/v3/index.json)
PushStatus=$?

if ((PushStatus != 0)); then
  echo Publish failed with status $PushOutput
  echo Publish Output:
  echo $PushOutput
  exit $PushStatus
fi

echo Push succeeded.


#Delete package

rm $PackageFile

echo Package file deletion succeeded.