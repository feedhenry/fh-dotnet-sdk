COPY /B FHSDK\bin\Debug\FHSDK.dll "Dist\lib\portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10\"
COPY /B FHSDK\bin\Debug\FHSDK.xml "Dist\lib\portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10\"

COPY /B FHSDKPhone\Bin\Debug\FHSDK.dll Dist\lib\wp80\
COPY /B FHSDKPhone\Bin\Debug\FHSDK.xml Dist\lib\wp80\
COPY /B FHSDKPhone\Bin\Debug\FHSDKPhone.dll Dist\lib\wp80\
COPY /B FHSDKPhone\Bin\Debug\FHSDKPhone.xml Dist\lib\wp80\

COPY /B FHSDKPortable\bin\Debug\FHSDK.dll "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDK.XML "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDKPortable.dll "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDKPortable.XML "Dist\lib\portable-win81+wpa81\"

COPY /B FHXamarinAndroidSDK\bin\Debug\FHSDK.dll Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHSDK.xml Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHXamarinAndroidSDK.dll Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHXamarinAndroidSDK.xml Dist\lib\monoandroid\

REM copy source
IF not exist "Dist\src\FHSDK\" (md "Dist\src\FHSDK\")
IF not exist "Dist\src\FHSDKPhone\" (md "Dist\src\FHSDKPhone\")
IF not exist "Dist\src\FHSDKPortable\" (md "Dist\src\FHSDKPortable\")
IF not exist "Dist\src\FHXamarinAndroidSDK\" (md "Dist\src\FHXamarinAndroidSDK\")
IF not exist "Dist\src\FHXamarinIOSSDK\" (md "Dist\src\FHXamarinIOSSDK\")



XCOPY FHSDK\*.cs Dist\src\FHSDK /sy
XCOPY FHSDKPhone\*.cs Dist\src\FHSDKPhone /sy
XCOPY FHSDKPortable\*.cs Dist\src\FHSDKPortable /sy
XCOPY FHXamarinAndroidSDK\*.cs Dist\src\FHXamarinAndroidSDK /sy
XCOPY FHXamarinIOSSDK\*.cs Dist\src\FHXamarinIOSSDK /sy

REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHSDK.dll Dist\lib\monotouch\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHSDK.xml Dist\lib\monotouch\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHXamarinIOSSDK.dll Dist\lib\monotouch\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHXamarinIOSSDK.xml Dist\lib\monotouch\
