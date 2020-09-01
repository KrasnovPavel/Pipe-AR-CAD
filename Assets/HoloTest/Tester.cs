// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if ENABLE_WINMD_SUPPORT
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
#endif

namespace HoloTest
{
    /// <summary> Компонент запускающий тесты при старте программы. </summary>
    public class Tester : MonoBehaviour
    {
        /// <summary> Выполнять тесты при старте программы? </summary>
        [Tooltip("Выполнять тесты при старте программы?")]
        public bool RunTests;

        #region Unity event functions

        private void Start()
        {
            if (RunTests) RunUnitTests();
        }

        #endregion

        #region Private definitions

        /// <summary> Запускает все тесты. </summary>
#pragma warning disable 1998
        private async void RunUnitTests()
#pragma warning restore 1998
        {
            Debug.Log("=========================== Testing started. ==========================");
            Assembly[] assemblies;
#if ENABLE_WINMD_SUPPORT
            assemblies = await GetAssemblies();
#else
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
#endif

            var testCases = assemblies
                            .SelectMany(a => a.GetTypes())
                            .Where(IsTestClass)
                            .SelectMany(t => t.GetMethods())
                            .Where(m => m.GetCustomAttributes(typeof(HoloTestCaseAttribute)).Any())
                            .Select(m=> new TestCase
                            {
                                Namespace = m.DeclaringType?.Namespace,
                                TestName = m.Name,
                                TypeName = m.DeclaringType?.Name,
                                TestFunction = delegate { m.Invoke(null, null); }
                            })
                            .ToList();
            
            // Добавляем тесты сгенерированные автоматически
            testCases.AddRange(assemblies
                .SelectMany(a => a.GetTypes())
                .Where(IsTestClass)
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(HoloTestGeneratorAttribute)).Any())
                .SelectMany(m =>
                {
                    return (m.Invoke(null, null) as IEnumerable<Action>)?.Select(a => new TestCase
                    {
                        Namespace = m.DeclaringType?.Namespace,
                        TestName = $"{m.Name}-{a.GetHashCode()}",
                        TypeName = m.DeclaringType?.Name,
                        IsGenerated = true,
                        TestFunction = a
                    });
                }));
            
            foreach (var test in testCases)
            {
                test.Run();
                switch (test.Result)
                {
                    case TestResult.DidNotRun:
                        Debug.LogWarning(test);
                        break;
                    case TestResult.Failed:
                        Debug.LogError(test);
                        break;
                    case TestResult.Passed:
                        Debug.Log(test);
                        break;
                }
            }

            var failedTests = testCases.Where(t => t.Result == TestResult.Failed).ToList();
            if (failedTests.Any())
            {
                Debug.Log($"================== Tests failed: {failedTests.Count} ==================");
                foreach (var test in failedTests)
                {
                    Debug.Log(test);
                }
                Debug.Log("=========================== Testing ended. ==========================");
            }
            else
            {
                Debug.Log("=========================== All Tests Passed! ==========================");
            }
        }

        /// <summary> Проверяет является ли указанный тип тестовым классом. </summary>
        /// <param name="type"> Проверяемый тип. </param>
        private bool IsTestClass(Type type)
        {
#if ENABLE_WINMD_SUPPORT
            return type.GetTypeInfo().GetCustomAttributes(typeof(HoloTestClassAttribute)).Any();
#else
            return type.GetCustomAttributes(typeof(HoloTestClassAttribute)).Any();
#endif
        }
        
#if ENABLE_WINMD_SUPPORT
        /// <summary> Выполняет поиск используемых приложением сборок в UWP. </summary>
        private async Task<Assembly[]> GetAssemblies()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            List<Assembly> assemblies = new List<Assembly>();
            foreach (Windows.Storage.StorageFile file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll")
                {
                    try
                    {
                        AssemblyName name = new AssemblyName
                        {
                            Name = Path.GetFileNameWithoutExtension(file.Name)
                        };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                    catch {}
                }
            }
            return assemblies.ToArray();
        }
#endif

        #endregion
    }
}