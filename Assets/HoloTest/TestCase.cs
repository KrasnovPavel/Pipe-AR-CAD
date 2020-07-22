// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace HoloTest
{
    /// <summary> Возможные результаты теста. </summary>
    internal enum TestResult
    {
        /// <summary> Тест не запускался. </summary>
        DidNotRun,
        /// <summary> Тест провален. </summary>
        Failed,
        /// <summary> Тест пройден. </summary>
        Passed
    }
    
    /// <summary> Класс, содержащий информацию о тесте. </summary>
    internal class TestCase
    {
        /// <summary> Пространство имён теста. </summary>
        public string Namespace;
        
        /// <summary> Класс теста. </summary>
        public string TypeName;
        
        /// <summary> Название теста. </summary>
        public string TestName;
        
        /// <summary> Результат теста. </summary>
        public TestResult Result = TestResult.DidNotRun;
        
        /// <summary> Длительность теста. </summary>
        public TimeSpan? TestDuration;
        
        /// <summary> Функция для запуска теста. </summary>
        public Action TestFunction;
        
        /// <summary> Сообщение об ошибке. </summary>
        public string ErrorMassage = "";

        /// <summary> Запускает тест. </summary>
        public void Run()
        {
            var start = DateTime.Now;

            try
            {
                TestFunction?.Invoke();
            }
            catch (Exception e)
            {
                ErrorMassage = e.InnerException?.Message;
                Result = TestResult.Failed;
                TestDuration = DateTime.Now - start;
                return;
            }
            
            Result = TestResult.Passed;
            TestDuration = DateTime.Now - start;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Namespace} {TypeName} {TestName} {TestDuration} {Result} {ErrorMassage}";
        }
    }
}