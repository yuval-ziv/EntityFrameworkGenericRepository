#!/bin/bash
if [[ -z "$1" ]] || [[ -z "$2" ]] || [[ -z "$3" ]] || [[ -z "$4" ]] || [[ -z "$5" ]]
then
  echo "ERROR: You must specify either a get, major, minor, patch or none argument, a project file, a nuget api key, path to bin directory and a build configuration."
  echo "Usage: $0 (get|major|minor|patch) MyProject.csproj nugetApiKey \\path\\to\\bin"
  exit 1
fi

if [[ ! -w $2 ]]
then
  echo "ERROR: The project file '$2' either does not exist, or is not writable."
  exit 2
fi

if [[ ! -w $4 ]]
then
  echo "ERROR: The path to bin/release directory '$4' either does not exist, or is not writable."
  exit 2
fi

Version=$(sed -n 's/.*<Version>\(.*\)<\/Version>.*/\1/p' $2)
echo Old Version: $Version
if [ -z "$Version" ]
then
  echo "ERROR: Could not find a <Version/> tag in the project file '$2'."
  echo "Please add one in between the <Project><PropertyGroup> tags and try again."
  exit 3
fi

BuildOutput=$(dotnet build --configuration $5)
BuildStatus=$?

if ((BuildStatus != 0)); then
  echo Build failed with status $BuildStatus.
  echo Build Output:
  echo $BuildOutput
  exit $BuildStatus
fi

echo Build succeeded.


TestOutput=$(dotnet test --configuration Release)
TestStatus=$?

if ((TestStatus != 0)); then
  echo Test failed with status $TestStatus.
  echo Test Output:
  echo $TestOutput
  exit $TestStatus
fi

echo Test succeeded.

VersionParts=(${Version//./ })
case "$1" in
  get) echo $Version; exit 0 ;;
  major) ((VersionParts[0]++)) ;;
  minor) ((VersionParts[1]++)) ;;
  patch) ((VersionParts[2]++)) ;;
  none) (()) ;;
  *)
    echo "ERROR: Invalid SemVer position name supplied, '$1' was not understood."
    echo "Usage: $0 (-get|major|minor|patch) $2"
    exit 4
esac

NewVersion="${VersionParts[0]}.${VersionParts[1]}.${VersionParts[2]}"
sed -i -e "s/<Version>$Version<\/Version>/<Version>$NewVersion<\/Version>/g" $2
echo New Version: $NewVersion

echo Enter release notes:
read -r ReleaseNotes

sed -i -e "s/<PackageReleaseNotes>\(.*\)<\/PackageReleaseNotes>/<PackageReleaseNotes>$ReleaseNotes<\/PackageReleaseNotes>/g" $2

PackOutput=$(dotnet pack --configuration Release)
PackStatus=$?

if ((PackStatus != 0)); then
  echo Pack failed with status $PackStatus
  echo Pack Output:
  echo $PackOutput
  exit $PackStatus
fi

echo Pack succeeded.

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

rm $PackageFile

echo Package file deletion succeeded.