
# .NET console app creator, compiler and executor

A simple .NET console app executor, initially builded for compile and execute my AoC solutions.
Uses .NET SDK and system processes to do the work.

+ Maximum allowed file size and destination directory are managable via appsettings.json.
+ Swagger is available in development mode


## API Reference

#### Compile and execute simple block of code

```http
  POST /api/compiler/run-code
```

| Parameter | Type     | Description                |
| :-------- | :------- | :------------------------- |
| `code` | `string` | **Required**. Your block of code |

#### Compile and execute simple block of code with input data

```http
  POST /api/compiler/run-code-with-input
```

| Parameter | Type     | Description                       |
| :-------- | :------- | :-------------------------------- |
| `code`      | `string` | **Required**. Your block of code |
| `inputFile` | `file` | **Required**. Your file with input data, only .txt format |


