# Plan for the summer term

## Re-organize your repository

Move your work from winter into a subfolder called *Source_Winter*.
Create a folder called *Source_Summer*.
Your folder structure should look like this:

```
📁 (your repo)
    ├──.gitignore
    ├──📂 Docs
    ├──📂 Source_Winter
    └──📂 Source_Summer
        ├──📂 <yourApp>.Application
        ├──📂 <yourApp>.Client
        ├──📂 <yourApp>.Test
        ├──📂 <yourApp>.Webapi
        ├──<yourApp>.sln
        ├──Dockerfile
        ├──start_server.cmd
        └──deploy_app.sh
```

Make your repository public.
Copy your model classes from the application project in *Source_Winter* into your current application project.
*deploy_app.sh* is available at https://github.com/schletz/Wmc/blob/main/32_Vuejs/deploy_app.sh

## Scaffolding your project

Scaffold your project with the following commands:

**Windows**
```
SET PROJECT_NAME=WerAllesKopiertIstDumm
md %PROJECT_NAME%
cd %PROJECT_NAME%
md %PROJECT_NAME%.Application
md %PROJECT_NAME%.Webapi
md %PROJECT_NAME%.Test
cd %PROJECT_NAME%.Application
dotnet new classlib -f net6.0
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 6.*
dotnet add package Bogus --version 34.*
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.*
cd ..\%PROJECT_NAME%.Webapi
dotnet new webapi -f net6.0
dotnet add reference ..\%PROJECT_NAME%.Application
cd ..\%PROJECT_NAME%.Test
dotnet new xunit -f net6.0
dotnet add reference ..\%PROJECT_NAME%.Application
cd ..
dotnet new sln
dotnet sln add %PROJECT_NAME%.Webapi
dotnet sln add %PROJECT_NAME%.Application
dotnet sln add %PROJECT_NAME%.Test
npm init vue@3 %PROJECT_NAME%.Client
```

The last command scaffolds a Vue.js Application.
You can also use Angular or React instead of Vue.js.
If you are using use the scaffolder of Vue.js, use the following settings:

```
Vue.js - The Progressive JavaScript Framework

√ Project name: ... <your-app>-client
√ Add TypeScript? ...                                         No
√ Add JSX Support? ...                                        No
√ Add Vue Router for Single Page Application development? ... Yes
√ Add Pinia for state management? ...                         No
√ Add Vitest for Unit Testing? ...                            No
√ Add Cypress for both Unit and End-to-End testing? ...       No
√ Add ESLint for code quality? ...                            Yes
√ Add Prettier for code formatting? ...                       No
```

After scaffolding **remove** the *ImplicitUsings* directive from all your project files.

## Grading scale for .NET implementations

> *Create issues for each grade level in you repository!*

### ASP.NET Backend

#### Genügend

- Scaffold your application.
- Seed your database with fake data.
- Add an ASP.NET Core controller for one entity with foreign keys (e. g. */api/photos*).
- Implement a GET Route */api/photos*. The result is HTTP 200 with a list of all photos.
  Use projection to send base information of the foreign entities (e. g. photographer).

More Informations: 
- https://github.com/schletz/Wmc/blob/main/32_Vuejs/01_Backend.md
- https://github.com/schletz/Wmc/blob/main/32_Vuejs/02_Controller.md
- https://github.com/schletz/Wmc/blob/main/32_Vuejs/03_Database.md

### Befriedigend

- Implement CRUD Operations for the entity in your controller (*photos* is only an example!):
  - POST */api/photos*
  - PUT */api/photos/{id}*
  - DELETE */api/photos/{id}*
- Define a record as DTO Class for the incoming data.
- You can use the internal key for the foreign key.

More Information: https://github.com/schletz/Wmc/blob/main/32_Vuejs/04_Crud.md

### Gut

- Decorate your DTO record with validation annotations.
- Your record should implement *IValidatableObject* to check the validity of your foreign key.
- Add a Guid property of type guid to your model classes. Make sure to configure your guid property as an alternate key. See [Eine Datenbank mit EF Core erstellen](https://github.com/schletz/Wmc/blob/main/32_Vuejs/03_Database.md#eine-datenbank-mit-ef-core-erstellen).
- To avoid sending the internal (auto increment) key, use the guid for the primary and foreign key. Do not send the internal key anymore.

### Sehr gut

- Inject the ASP.NET logger and identify and log business critical scenarios.
- Annotate your controller with *ProducesResponseType*, see https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-6.0.
- Implement Swagger and OpenAPI wo send information about your controller, see https://khalidabuhakmeh.com/generate-aspnet-core-openapi-spec-at-build-time.

### SPA Frontend

#### Genügend

- Create a SPA application with a client-side router.
- Create a view (table) to display a list of the data. Call your controller that you wrote in part 1.

#### Befriedigend

- Add an edit and delete button to your table rows. When a user clicks the delete button, a confirmation dialog should appear.
- When the user clicks on the edit button, a dialog with an edit form should appear. See https://www.w3schools.com/howto/howto_css_modals.asp for a CSS example. Of course you can use packages for modal dialogs.
- Foreign keys should rendered as a dropdown list. Providing a text field for the foreign key is not allowed.
- Your controller creates/adds/deletes the entity in your database.
- Your edit dialog is an own component.
- If an error occurs (return code not 2xx) you have to show an alert.

#### Gut

- Add a nice validation to your edit dialog. If a value is invalid, an error message should be shown at the form control.
- Use the server side validation (validation annotations, IValidateObject) to validate your data.
- Use the information in the HTTP 400 response to grab the error message.

See https://github.com/schletz/Wmc/blob/main/32_Vuejs/06_VuejsClient.md#formulare-und-validierung

#### Sehr gut

- Add a JWT authentication, see https://github.com/schletz/Wmc/blob/main/32_Vuejs/05_JwtAuthentication.md
- Secure your controller(s) with *Authorize* Annotations.
- Secure your client application. You can list and edit your data if you are authenticated.
- Create a Dockerfile to deploy your app, see https://github.com/schletz/Wmc/blob/main/32_Vuejs/08_Deployment_Docker.md 
- Customize *deploy_app.sh* to set proper environment variables, see https://github.com/schletz/Wmc/blob/main/32_Vuejs/09_AzureDeployment.md

