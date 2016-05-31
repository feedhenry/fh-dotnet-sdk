1) Pre-requisite

 * register to [NuGet central repo](https://www.nuget.org/)
 * one NuGet owner of `FH.SDK` should have added you to the list of owners to grant you access

Publishing to NuGet requires you to build for each platforms. Create a branch with your new release version.

2) Build on iOS:

 * open Xamarin Studio, open solution: `FHSDK.sln`
 * choose `Release` on the top bar schema selection
  select `FHXamarinIOSSDK` project, clean and build
 * open a command line and run:
```
$ copyIOSLibs.sh
```
* check a new dll file was added in `Dist/lib/xamarinios10/FHXamarinIOSSDK.dll`
* commit

3) Build on Windows/Android:

* open Visual Studio, open solution: `FHSDK.sln`
* choose `Release` on the top bar schema selection
* select `FHXamarinAndroidSDK` project, clean and build
* select `FHSDK` project, clean and build
* open a command line and run:
```
$ copyLibs.bat
```
* check new dll files were added
* commit

4) Bump version
In `Dist/FHSDK.nuspec` file, change the version

5) Push to NuGet

* Make sure nuget.exe is installed and on the PATH. Download From [here](http://nuget.org/nuget.exe)
* Run:

````batch
nuget setApiKey <api key>
````

```bash
find .. -name '*.cs' -print > copy-src.sh
```

open copy-src.sh in a good text editor and use regex to find `\.\./(.*)` and replace with ``mkdir -p src/`dirname $1` && cp ../$1 src/$1``

Excecute the created script and remove the tests source and xamarin, cause tests are not in the nuget package and xamarin can't be debugged:

```bash
chmod +x copy-src.sh
./copy-src.sh
rm -rf src/tests* src/xamarin
```

After updating the nuspec with the right version you can pack the release and upload:

```bash
nuget pack FHSDK.nuspec -Symbols
nuget push FH.SDK.<version>.nupkg
```

6) Merge the branch
