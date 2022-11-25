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

![](http://www.plantuml.com/plantuml/svg/bLH1Rziy3BtdLnW-zljow3KeZ0PT2XJ8iYqsUXemQ0xdUB2bLQBR0jl--sY58gmeCxgS8fBlyP5Fygr3PN7ekqNTAUUodUeXktbMtG2P-VeTQYoodukCVlzbtrhhK5C1dpl5nzyCha3LHx0269FPYZW50hrgEmgLQ5lT4310hfRBubJ8N6LzDARp_uNF7Or59XM61UYXp_Qc0wxRVTY_Ns__z__ljkko_BBVVDnloiskMbSFZnl2X1uu743Zc_Loq8wzLhvb43OHkvfAfrwXYKmeaVmgpymOpQ_p06AAl9XovX69RomZ3DwakHMnMKOtSLpMy6a0XwtHMhMMp5TDOw5H2Epyx7oSPnMhe8VZoEwkmjNWk7dHFifZ1HPKIFfYFQKFqe-BdZWv2jcs3cz2TJ4E547_OS5pG1RfGIVbi-KygI_d1Sm3U6KiwdJ8pxqHAn1UyTpRfmbvia2kgIqeX0Dbtoc4gfKhhhHx0IlhNFjZj4mlzuhbnkIZw1lfxpxh8XOZS-mLajvachxLfmmHFMY41cp8gDSa8pVstW9Kj8Y8Zr5Rq0qUOzHPz-NmO_cha4aMCLrQ4DgUTYFDmgIJZKzCUbJTaBp2LdCyaXVG_avmbtYpjDXWbAmqVIQaxC8ZkBvi7cDajXhRPuZoCgZokSfCBuzuSTSP1uVBm9foDFm8cc8bIS1xQ_h9bu1eBoWHGrBvQVkvQjPSKglBYXq_Tb7qfYT6gbDAxsRwN98NyXTa_DrXNVdY5lHXwBi_)
<sup>[Source](http://www.plantuml.com/plantuml/uml/bLH1Rziy3BtdLnW-zljow3KeZ0PT2XJ8iYqsUXemQ0xdUB2bLQBR0jl--sY58gmeCxgS8fBlyP5Fygr3PN7ekqNTAUUodUeXktbMtG2P-VeTQYoodukCVlzbtrhhK5C1dpl5nzyCha3LHx0269FPYZW50hrgEmgLQ5lT4310hfRBubJ8N6LzDARp_uNF7Or59XM61UYXp_Qc0wxRVTY_Ns__z__ljkko_BBVVDnloiskMbSFZnl2X1uu743Zc_Loq8wzLhvb43OHkvfAfrwXYKmeaVmgpymOpQ_p06AAl9XovX69RomZ3DwakHMnMKOtSLpMy6a0XwtHMhMMp5TDOw5H2Epyx7oSPnMhe8VZoEwkmjNWk7dHFifZ1HPKIFfYFQKFqe-BdZWv2jcs3cz2TJ4E547_OS5pG1RfGIVbi-KygI_d1Sm3U6KiwdJ8pxqHAn1UyTpRfmbvia2kgIqeX0Dbtoc4gfKhhhHx0IlhNFjZj4mlzuhbnkIZw1lfxpxh8XOZS-mLajvachxLfmmHFMY41cp8gDSa8pVstW9Kj8Y8Zr5Rq0qUOzHPz-NmO_cha4aMCLrQ4DgUTYFDmgIJZKzCUbJTaBp2LdCyaXVG_avmbtYpjDXWbAmqVIQaxC8ZkBvi7cDajXhRPuZoCgZokSfCBuzuSTSP1uVBm9foDFm8cc8bIS1xQ_h9bu1eBoWHGrBvQVkvQjPSKglBYXq_Tb7qfYT6gbDAxsRwN98NyXTa_DrXNVdY5lHXwBi_)</sup>
