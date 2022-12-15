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

## The model

![](http://www.plantuml.com/plantuml/svg/bLJ1Rfj04BtxAn37j3xaQbaeLkf8aLot1PHJfMgBfvWATfjbI2Ar_VSEC3jblHIblXXcvhrvCs-NcnQLnQwfLqMjsZOwgWQYxPEgEuZCr-zGO1BzN4NqUnDzgsoBcWhulLRy-fl1EQZY35O0VJBQ4AT0e55LJQ4CRQLB0Zh8rNgTZ8NCbHLdO-hnsV6rPsEHIOLX1RfhejJKm7N73-dxtU7jy7nxs6NPbtJ_CTrd-sE-o-yVzeHnFR0xWSPNw-MWxNjjXfPEs4JiPYgTUhacCg54ugiuCdqqleuTY2dYPCeP-oHzOncX6qnQMX6RPNJfnsKDdpfeiJ9QgygI-QeiBPGAWPrVd8_ph6BZz70SsTsDEneSDyzwYF9OZWKLaZvVJpO4wST5ZvpiXHog5hUkEka7yg3_i-0588iSGOFonNAUT2pd1Im2U6KiQdo9BxqHAn1UyDBRHu_yOe5SKrXG22VAlgF_l98ZhdJx35RMkHjUlMKESgzOha_UYzx-zUIpJdmnCaUgaFG6qpIL7ZD4TAyHIh0kerw2Z9pOEmkGqo8yFaOTG9Tuzb4NtTVTZ_MlH2PP-NHXWMixxaQQkKad6v-OzA3gBhY5bUQu9y-XnpF1NRpDqcATKRBIy9gGifbBS3rl7cDajWAh9_3oCgWQShcP7XxnuhOsBPpcWGNbQFWUDCLAae1trZIJBm7HpYWHGqBvGVkbQlbgKgjvnIrVTb7qgYj6gaFAqStWkWGtv1z8_xl3kkAvUPVARq2VkgR-0m00)
<sup>[Source](http://www.plantuml.com/plantuml/uml/bLJ1Rfj04BtxAn37j3xaQbaeLkf8aLot1PHJfMgBfvWATfjbI2Ar_VSEC3jblHIblXXcvhrvCs-NcnQLnQwfLqMjsZOwgWQYxPEgEuZCr-zGO1BzN4NqUnDzgsoBcWhulLRy-fl1EQZY35O0VJBQ4AT0e55LJQ4CRQLB0Zh8rNgTZ8NCbHLdO-hnsV6rPsEHIOLX1RfhejJKm7N73-dxtU7jy7nxs6NPbtJ_CTrd-sE-o-yVzeHnFR0xWSPNw-MWxNjjXfPEs4JiPYgTUhacCg54ugiuCdqqleuTY2dYPCeP-oHzOncX6qnQMX6RPNJfnsKDdpfeiJ9QgygI-QeiBPGAWPrVd8_ph6BZz70SsTsDEneSDyzwYF9OZWKLaZvVJpO4wST5ZvpiXHog5hUkEka7yg3_i-0588iSGOFonNAUT2pd1Im2U6KiQdo9BxqHAn1UyDBRHu_yOe5SKrXG22VAlgF_l98ZhdJx35RMkHjUlMKESgzOha_UYzx-zUIpJdmnCaUgaFG6qpIL7ZD4TAyHIh0kerw2Z9pOEmkGqo8yFaOTG9Tuzb4NtTVTZ_MlH2PP-NHXWMixxaQQkKad6v-OzA3gBhY5bUQu9y-XnpF1NRpDqcATKRBIy9gGifbBS3rl7cDajWAh9_3oCgWQShcP7XxnuhOsBPpcWGNbQFWUDCLAae1trZIJBm7HpYWHGqBvGVkbQlbgKgjvnIrVTb7qgYj6gaFAqStWkWGtv1z8_xl3kkAvUPVARq2VkgR-0m00)</sup>
