COPY /B FHSDK\bin\Release\FHSDK.dll "Dist\lib\portable-net45+wp80+wp81+win8+wpa81\"
COPY /B FHSDK\bin\Release\FHSDK.xml "Dist\lib\portable-net45+wp80+wp81+win8+wpa81\"

COPY /B FHSDKPhone\Bin\Release\FHSDK.dll Dist\lib\wp80\
COPY /B FHSDKPhone\Bin\Release\FHSDK.xml Dist\lib\wp80\
COPY /B FHSDKPhone\Bin\Release\FHSDKPhone.dll Dist\lib\wp80\
COPY /B FHSDKPhone\Bin\Release\FHSDKPhone.xml Dist\lib\wp80\

COPY /B FHSDKPortable\bin\Release\FHSDK.dll Dist\lib\wp81\
COPY /B FHSDKPortable\bin\Release\FHSDK.xml Dist\lib\wp81\
COPY /B FHSDKPortable\Bin\Release\FHSDKPortable.dll Dist\lib\wp81\
COPY /B FHSDKPortable\Bin\Release\FHSDKPortable.xml Dist\lib\wp81\

COPY /B FHSDKPortable\bin\Release\FHSDK.dll Dist\lib\wpa81\
COPY /B FHSDKPortable\bin\Release\FHSDK.xml Dist\lib\wpa81\
COPY /B FHSDKPortable\bin\Release\FHSDKPortable.dll Dist\lib\wpa81\
COPY /B FHSDKPortable\bin\Release\FHSDKPortable.XML Dist\lib\wpa81\

COPY /B FHSDKPortable\bin\Release\FHSDKPortable.dll "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Release\FHSDKPortable.XML "Dist\lib\portable-win81+wpa81\"

COPY /B FHXamarinAndroidSDK\bin\Release\FHSDK.dll Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Release\FHSDK.xml Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Release\FHXamarinAndroidSDK.dll Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Release\FHXamarinAndroidSDK.xml Dist\lib\monoandroid\

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

REM COPY /B xamarin\FHXamarinIOSSDK\bin\Release\FHSDK.dll Dist\lib\monotouch\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Release\FHSDK.xml Dist\lib\monotouch\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Release\FHXamarinIOSSDK.dll Dist\lib\monotouch\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Release\FHXamarinIOSSDK.xml Dist\lib\monotouch\
