﻿using Microsoft.Extensions.DependencyInjection;
using SqlSugar.IOC;
using System;
using Newtonsoft.Json;
using SqlSugar;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

public static class SugarServiceCollectionExtensions
{
    internal static List<IocConfig> configs = new List<IocConfig>();
    public static void AddSqlSugar(this IServiceCollection server, IocConfig ioc)
    {
        configs.Add(ioc);
    }
    public static void AddSqlSugar(this IServiceCollection server, List<IocConfig> iocList)
    {
        configs.AddRange(iocList);
    }

    public static void AddIoc(this IServiceCollection serve, object webObject, string assemblyName,Func<Type,bool> classFilter) 
    {
        Assembly assembly = null;
        var mainAssembly= webObject.GetType().Assembly;
        if (mainAssembly.GetName().Name == assemblyName)
        {
            assembly = mainAssembly;
        }
        else
        {
            var name = mainAssembly.GetReferencedAssemblies().Where(it => it.Name == assemblyName).FirstOrDefault();
            if (name != null)
            {
                assembly = Assembly.Load(name);
            }
        }
        if (assembly == null)
        {
            throw new Exception("未找到程序集" + assembly.FullName+" "+mainAssembly.FullName+"未引用，可能是大小写或者写错。");
        }
        var types = assembly.GetTypes().Where(classFilter).ToList();
        foreach (var item in types.Where(it=>it.IsInterface==false&&it.IsClass==true))
        {
            serve.AddScoped(item);
            foreach (var  face in item.GetInterfaces())
            {
                serve.AddScoped(face, item);
            }
        }
    }
}

 

