* Make sure FHSDK, FHSDKPhone, FHXamarinAndroidSDK and FHXamarinIOSSDK projects are building correctly. 
* Run copyLibs.bat
* Bump version in Dist/FHSDK.nuspec file
* Make sure nuget.exe is installed and on the PATH. Download From [here](http://nuget.org/nuget.exe)
* Run

````batch
nuget setApiKey <api key>
````

```bash
find .. -name '*.cs' -print > copy-src.sh
```

open copy-src.sh in a good text editor and use regex to find `\.\./(.*)` and replace with ``mkdir -p src/`dirname $1` && cp ../$1 src/$1``

Excecute the created script and remove the tests source and xamarin, cause tests are not in the nuget package and xamarin can't be debugged

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
