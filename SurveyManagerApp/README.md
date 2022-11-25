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

![](http://www.plantuml.com/plantuml/svg/ZLN1Zfim4Btp5HR7j3di5KLeesrMMYbDMq0zLQfSCWLNO7VDi8tKzjyxk6CM4uJ29NXctfitRnnntw6qsBTD59MDx3fnaYs8tOjiUX3csmyeCHM_8q7F6_5TsGur5V2vaNpyo-02P5c37G53KYIEaoFGIjLGA4UhT4L03zbijwahz4BoiZQcSUyZNrSRwpfE6IBGVIioqm3NdJvax_V7j-xzuRZFyw_PuMDso0-dObyyVJuGnlV0_WmQRzRBGJlqIbnBBsmYDfbAfrxSP3AXZyIRM9WX6j_53F8KSJhbZ4CIrzYFqBibhLd4opAw2kEZ3Pzww50PhQMoj7nPLHOgYS2RNvsFywmYyNeuZhpTn5yDZfjVsaLvBC-24abVg2TtGNguw9cJWv2Zwd3dgzDXe03g3DxSPl0QaBrt8ATyjPmdTULim2g0BMD-TZAUxsPS1S85vxjz3iXd1ivgyi1aFdDDQO4EPyg-ezz2jJ3rD5GT3aJQTw-h-QBJKDCuJYQHP5yDrIhzFqD4JngX0kipydAL6I_kemKeo8-0Zr57q1NM8MhMVTl_tFv9nqcYaEwwWDikt4zgl73xxfRjOendsvUeNY24-ZcaonKct0W37nhJmNaPM5AEHXr0Komd7V3HcdRo5oJQ1IMX1O5yRdzZTNGF-ZnyP_u1)
<sup>[Source](http://www.plantuml.com/plantuml/uml/ZLN1Zfim4Btp5HR7j3di5KLeesrMMYbDMq0zLQfSCWLNO7VDi8tKzjyxk6CM4uJ29NXctfitRnnntw6qsBTD59MDx3fnaYs8tOjiUX3csmyeCHM_8q7F6_5TsGur5V2vaNpyo-02P5c37G53KYIEaoFGIjLGA4UhT4L03zbijwahz4BoiZQcSUyZNrSRwpfE6IBGVIioqm3NdJvax_V7j-xzuRZFyw_PuMDso0-dObyyVJuGnlV0_WmQRzRBGJlqIbnBBsmYDfbAfrxSP3AXZyIRM9WX6j_53F8KSJhbZ4CIrzYFqBibhLd4opAw2kEZ3Pzww50PhQMoj7nPLHOgYS2RNvsFywmYyNeuZhpTn5yDZfjVsaLvBC-24abVg2TtGNguw9cJWv2Zwd3dgzDXe03g3DxSPl0QaBrt8ATyjPmdTULim2g0BMD-TZAUxsPS1S85vxjz3iXd1ivgyi1aFdDDQO4EPyg-ezz2jJ3rD5GT3aJQTw-h-QBJKDCuJYQHP5yDrIhzFqD4JngX0kipydAL6I_kemKeo8-0Zr57q1NM8MhMVTl_tFv9nqcYaEwwWDikt4zgl73xxfRjOendsvUeNY24-ZcaonKct0W37nhJmNaPM5AEHXr0Komd7V3HcdRo5oJQ1IMX1O5yRdzZTNGF-ZnyP_u1)</sup>
