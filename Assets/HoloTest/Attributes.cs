// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace HoloTest
{
    /// <summary> Атрибут, указывающий, что класс содержит тестовые функции. </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HoloTestClassAttribute : Attribute { }
    
    /// <summary> Атрибут, указывающий, что функция является тестовой. </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HoloTestCaseAttribute : Attribute { }
    
    /// <summary> Атрибут, указывающий, что функция является генератором тестов. </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HoloTestGeneratorAttribute : Attribute {}
}