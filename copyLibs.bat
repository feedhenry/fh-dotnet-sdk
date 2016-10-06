COPY /B FHSDK\bin\Debug\FHSDK.dll "Dist\lib\portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10\"
COPY /B FHSDK\bin\Debug\FHSDK.pdb "Dist\lib\portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10\"
COPY /B FHSDK\bin\Debug\FHSDK.xml "Dist\lib\portable-net45+win+wpa81+wp80+MonoAndroid10+xamarinios10+MonoTouch10\"

COPY /B FHSDKPhone\bin\Debug\FHSDK.dll Dist\lib\wp80\
COPY /B FHSDKPhone\bin\Debug\FHSDK.xml Dist\lib\wp80\
COPY /B FHSDKPhone\bin\Debug\FHSDK.pdb Dist\lib\wp80\
COPY /B FHSDKPhone\bin\Debug\FHSDKPhone.dll Dist\lib\wp80\
COPY /B FHSDKPhone\bin\Debug\FHSDKPhone.pdb Dist\lib\wp80\
COPY /B FHSDKPhone\bin\Debug\FHSDKPhone.xml Dist\lib\wp80\

COPY /B FHSDKPortable\bin\Debug\FHSDK.dll "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDK.pdb "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDK.XML "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDKPortable.dll "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDKPortable.pdb "Dist\lib\portable-win81+wpa81\"
COPY /B FHSDKPortable\bin\Debug\FHSDKPortable.XML "Dist\lib\portable-win81+wpa81\"

COPY /B FHXamarinAndroidSDK\bin\Debug\FHSDK.dll Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHSDK.pdb Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHSDK.xml Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHXamarinAndroidSDK.dll Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHXamarinAndroidSDK.pdb Dist\lib\monoandroid\
COPY /B FHXamarinAndroidSDK\bin\Debug\FHXamarinAndroidSDK.xml Dist\lib\monoandroid\

XCOPY FHSDK\*.cs Dist\src /sy
XCOPY FHSDKPhone\*.cs Dist\src /sy
XCOPY FHSDKPortable\*.cs Dist\src /sy
XCOPY FHXamarinAndroidSDK\*.cs Dist\src /sy
XCOPY FHXamarinIOSSDK\*.cs Dist\src /sy

REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHSDK.dll Dist\lib\xamarinios10\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHSDK.xml Dist\lib\xamarinios10\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHXamarinIOSSDK.dll Dist\lib\xamarinios10\
REM COPY /B xamarin\FHXamarinIOSSDK\bin\Debug\FHXamarinIOSSDK.xml Dist\lib\xamarinios10\
