# Survey Manager



## Scaffolding your application

```text
md SurveyManagerApp
cd SurveyManagerApp
md SurveyManagerApp.Application
md SurveyManagerApp.Webapi
md SurveyManagerApp.Test
cd SurveyManagerApp.Application
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 6.*
cd ..\SurveyManagerApp.Webapi
dotnet new webapi
dotnet add reference ..\SurveyManagerApp.Application
cd ..\SurveyManagerApp.Test
dotnet new xunit
dotnet add reference ..\SurveyManagerApp.Application
cd ..
dotnet new sln
dotnet sln add SurveyManagerApp.Webapi
dotnet sln add SurveyManagerApp.Application
dotnet sln add SurveyManagerApp.Test
start SurveyManagerApp.sln
```

After scaffolding change the properties in your csproj files to .net 6:

```xml
<PropertyGroup>
        <TargetFramework>net6.0</TargetFramework>
        <Nullable>enable</Nullable>
        <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

In your ASP.NET Core project remove OpenAPI and Swagger dependencies and fix some using directives
because we disabled *ImplicitUsings* to avoid namespace conflicts with your model classes.

## The Model

![](http://www.plantuml.com/plantuml/svg/ZLJ1Rjim3BtdAmmVi-NGQn0O3ReKA91bcstqD63G7CxnO4khJBS5jlrxQOKYB2KmwfD5yf7lFSg-xL5P7BfsKRMgxvE3wY3PFAjsWCJy_0qLPidVHKBFX-HNOtlKLC3dLl7nFuDBKDK9h036PBAcdWA1JZKjXGgqZQu9w27BrIex5tAleZePqxftwTUVZ7KJfnqMe8SkoKqBN7Vuad_UxZ-wzxlzjYX-vBkl-QxO7Sfj-V2u8uoVWSCHDBwRBmVjE6ljHdfY4xBhANMQvOU8GecaopGnOpIzIJs8MwJPj6SQD763HSBWbZHd4I_Bw3gCYmtV1kYnCLghnjBoLLrRg1K2RtvM7-UPnThpuJZoTjV-Qd3Sl6WNPLc-2oeaVY6VmWNfuQ8dJev4zasF6r-TZO82g3Du-Jw3vu3ilGCvvhFbhDILim6p0BQC-xEJQRmRMGN2AyQxVGgQHmP6DKLWCf-vfh90XoDbFoc4ikbyGkd-1QnOldN7mF1GKgYMX6raVlzTPo5XqPeh94qNYhj6dpFKw44Zr61zHhrUPEHMtrk0aimA-Z5g3xh6KuYAfg-6Fwj_cIXPnEtY0Z_sx8qV9aedZayM-QZQ0U9jDfhZ0JsPQyXadX8wIyRwHOcbbvyICRloeTp4omimegt2vXc2l0XLBbUQwUMHNTorfeVZTM15EH8_WgPOIHBmtffkyhUWjbUO10i9wSVZvwjlGH-7hdq3)
<sup>[Source](http://www.plantuml.com/plantuml/uml/ZLJ1Rjim3BtdAmmVi-NGQn0O3ReKA91bcstqD63G7CxnO4khJBS5jlrxQOKYB2KmwfD5yf7lFSg-xL5P7BfsKRMgxvE3wY3PFAjsWCJy_0qLPidVHKBFX-HNOtlKLC3dLl7nFuDBKDK9h036PBAcdWA1JZKjXGgqZQu9w27BrIex5tAleZePqxftwTUVZ7KJfnqMe8SkoKqBN7Vuad_UxZ-wzxlzjYX-vBkl-QxO7Sfj-V2u8uoVWSCHDBwRBmVjE6ljHdfY4xBhANMQvOU8GecaopGnOpIzIJs8MwJPj6SQD763HSBWbZHd4I_Bw3gCYmtV1kYnCLghnjBoLLrRg1K2RtvM7-UPnThpuJZoTjV-Qd3Sl6WNPLc-2oeaVY6VmWNfuQ8dJev4zasF6r-TZO82g3Du-Jw3vu3ilGCvvhFbhDILim6p0BQC-xEJQRmRMGN2AyQxVGgQHmP6DKLWCf-vfh90XoDbFoc4ikbyGkd-1QnOldN7mF1GKgYMX6raVlzTPo5XqPeh94qNYhj6dpFKw44Zr61zHhrUPEHMtrk0aimA-Z5g3xh6KuYAfg-6Fwj_cIXPnEtY0Z_sx8qV9aedZayM-QZQ0U9jDfhZ0JsPQyXadX8wIyRwHOcbbvyICRloeTp4omimegt2vXc2l0XLBbUQwUMHNTorfeVZTM15EH8_WgPOIHBmtffkyhUWjbUO10i9wSVZvwjlGH-7hdq3)</sup>
