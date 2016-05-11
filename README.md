# Grammophone.Ennoun.Web
This repository holds an ASP.NET MVC web application which offers services for the ancient Greek language using the 
[Grammophone.EnnounInference](https://github.com/grammophone/Grammophone.EnnounInference) engine and dictionary 
lookup using the [Grammophone.Lexica](https://github.com/grammophone/Grammophone.Lexica) system.

The [trained model file](https://onedrive.live.com/redir?resid=29F6439B040F2700!2450&authkey=!AP_jf2MulFRsB-w&ithint=file%2crar)
should be placed in folder `\Ennoun Training` of the drive where the web application is deployed, as specified in the `InferenceSetup.xaml` file found in folder `Grammophone.Ennoun.Web\Configuration`.

This application requires 64 bits and a minimum memory of 12GB RAM for a satisfactory trained model.
Please [enable 64 bits when debugging with IIS express](https://blogs.msdn.microsoft.com/rob/2013/11/14/debugging-vs2013-websites-using-64-bit-iis-express/), as this is not the default.

Please setup IIS so as not to recycle the application, otherwise recycling will cause the trained model to be loaded again resulting to 3 minutes of not accepting inference requests.

The repository contains submodules, so make sure you clone recursively.
