# DDD 战术框架脚手架命令行工具

[![Release Build](https://img.shields.io/github/actions/workflow/status/netcorepal/netcorepal-cloud-cli/release.yml?label=release%20build)](https://github.com/netcorepal/netcorepal-cloud-cli/.github/workflows/release.yml)
[![Preview Build](https://img.shields.io/github/actions/workflow/status/netcorepal/netcorepal-cloud-cli/preview.yml?label=preview%20build)](https://github.com/netcorepal/netcorepal-cloud-cli/.github/workflows/preview.yml)
[![NuGet](https://img.shields.io/nuget/v/NetCorePal.Cloud.CLI.Toolkit.svg)](https://www.nuget.org/packages/NetCorePal.Cloud.CLI.Toolkit)
[![MyGet Preview](https://img.shields.io/myget/netcorepal/vpre/NetCorePal.Cloud.CLI.Toolkit?label=preview)](https://www.myget.org/feed/netcorepal/package/nuget/NetCorePal.Cloud.CLI.Toolkit)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/netcorepal/netcorepal-cloud-cli/blob/main/LICENSE)

用于快速生成基于 [netcorepal-cloud-framework](https://github.com/netcorepal/netcorepal-cloud-framework) 项目的模板代码

## 📦 安装

### 从源码构建
```bash
# 在项目目录执行
dotnet build
dotnet pack -c Release -o ./nupkg
dotnet tool install --global --add-source ./nupkg NetCorePal.Cloud.CLI.Toolkit
```

### NuGet 安装
```bash
dotnet tool install --global NetCorePal.Cloud.CLI.Toolkit
```

## 🛠 使用指南
可以使用 --help 或 -h 查看帮助
```bash
ncp --help
```

### 命令结构
```bash
ncp new [command] -n <NAME>
```

| 命令   | 生成内容                          | 输出文件示例               |
|--------|---------------------------------|--------------------------|
| ar     | 聚合根类型                       | `{Name}.cs`             |
| cmd    | 命令+验证器+处理器三件套           | `{Name}Command.*.cs`     |
| repo   | 仓储接口与实现                    | `I{Name}Repository.cs` |
| de     | 领域事件                         | `{Name}DomainEvent.cs`   |
| deh    | 领域事件处理器                    | `{Name}.cs`  |

### 通用选项
| 参数               | 说明                     |
|--------------------|------------------------|
| `-n`, `--name`     | 实体名称（必填） |
| `-d`, `--output-directory`   | 输出目录（默认：当前目录）      |

### 使用示例

**生成命令模板**
```bash
ncp new cmd -n CreateProduct
```
