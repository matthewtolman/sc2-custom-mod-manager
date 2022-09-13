#rm -rf out/avalon && dotnet restore -r osx-x64 && dotnet publish SC2_Avalonia_UI -c:Release -o out/avalon --sc --nologo -r:osx-x64 -p:PublishSingleFile=true

rm -rf out/mac-app
mkdir -p out/mac-app/SC2\ CCM\ Avalon\ Edition/SC2\ CCM\ Avalon\ Edition.app/Contents/Frameworks
mkdir -p out/mac-app/SC2\ CCM\ Avalon\ Edition/SC2\ CCM\ Avalon\ Edition.app/Contents/MacOS
cp -R -f out/avalon/* out/mac-app/SC2\ CCM\ Avalon\ Edition/SC2\ CCM\ Avalon\ Edition.app/Contents/MacOS
cp -R -f out/avalon/*.dylib out/mac-app/SC2\ CCM\ Avalon\ Edition/SC2\ CCM\ Avalon\ Edition.app/Contents/Frameworks
cd "out/mac-app/"

APP_ENTITLEMENTS="Sc2 CCM Avalon EditionEntitlements.entitlements"
APP_SIGNING_IDENTITY="3rd Party Mac Developer Application: [***]"
INSTALLER_SIGNING_IDENTITY="3rd Party Mac Developer Installer: [***]"
APP_NAME="SC2 CCM Avalon Edition/SC2 CCM Avalon Edition.app"

productbuild --component SC2\ CCM\ Avalon\ Edition/SC2\ CCM\ Avalon\ Edition.app /Applicat ions --sign "$INSTALLER_SIGNING_IDENTITY" SC2\ CCM\ Avalon\ Edition.pkg