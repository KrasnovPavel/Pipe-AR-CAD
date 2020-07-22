// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
#if ENABLE_WINMD_SUPPORT
using System.Reflection;
#endif

namespace HoloTest
{
    /// <summary> Класс, содержащий функции проверок для тестирования. </summary>
    public static class Assert
    {
        /// <summary> Проверяет равенство двух чисел с плавающей запятой. </summary>
        /// <param name="first"> Первое число. </param>
        /// <param name="second"> Второе число. </param>
        /// <param name="eps"> Точность проверки. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreEqual(float first, float second, float eps = float.Epsilon)
        {
            if (Math.Abs(first - second) > eps)
            {
                throw new AssertFailedException($"AreEqual({first}, {second}, eps={eps})");
            }
        }

        /// <summary> Проверяет равенство двух целых чисел. </summary>
        /// <param name="first"> Первое число. </param>
        /// <param name="second"> Второе число. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreEqual(int first, int second)
        {
            if (first != second)
            {
                throw new AssertFailedException($"AreEqual({first}, {second})");
            }
        }

        /// <summary> Проверяет равенство двух объектов. </summary>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreEqual(object first, object second)
        {
            if (!first.Equals(second))
            {
                throw new AssertFailedException($"AreEqual({first}, {second})");
            }
        }

        /// <summary> Проверяет равенство двух строк. </summary>
        /// <param name="first"> Первая строка. </param>
        /// <param name="second"> Вторая строка. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreEqual(string first, string second)
        {
            if (first != second)
            {
                throw new AssertFailedException($"AreEqual({first}, {second})");
            }
        }

        /// <summary> Проверяет неравенство двух чисел с плавающей запятой. </summary>
        /// <param name="first"> Первое число. </param>
        /// <param name="second"> Второе число. </param>
        /// <param name="eps"> Точность проверки. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreNotEqual(float first, float second, float eps = float.Epsilon)
        {
            if (Math.Abs(first - second) < eps)
            {
                throw new AssertFailedException($"AreNotEqual({first}, {second}, eps={eps})");
            }
        }

        /// <summary> Проверяет неравенство двух целых чисел. </summary>
        /// <param name="first"> Первое число. </param>
        /// <param name="second"> Второе число. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreNotEqual(int first, int second)
        {
            if (first == second)
            {
                throw new AssertFailedException($"AreNotEqual({first}, {second})");
            }
        }

        /// <summary> Проверяет неравенство двух объектов. </summary>
        /// <param name="first"> Первый объект. </param>
        /// <param name="second"> Второй объект. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreNotEqual(object first, object second)
        {
            if (!first.Equals(second))
            {
                throw new AssertFailedException($"AreNotEqual({first}, {second})");
            }
        }

        /// <summary> Проверяет неравенство двух строк. </summary>
        /// <param name="first"> Первая строка. </param>
        /// <param name="second"> Вторая строка. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void AreNotEqual(string first, string second)
        {
            if (first == second)
            {
                throw new AssertFailedException($"AreEqual({first}, {second})");
            }
        }

        /// <summary> Всегда проваливает тест. </summary>
        /// <param name="message"> Сообщение об ошибке. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void Fail(string message = "")
        {
            throw new AssertFailedException($"Failed ({message})");
        }

        /// <summary> Проверяет истинность аргумента. </summary>
        /// <param name="value"> Проверяемый аргумент. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void IsTrue(bool value)
        {
            if (!value)
            {
                throw new AssertFailedException($"IsTrue");
            }
        }

        /// <summary> Проверяет ложность аргумента. </summary>
        /// <param name="value"> Проверяемый аргумент. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void IsFalse(bool value)
        {
            if (value)
            {
                throw new AssertFailedException($"IsFalse");
            }
        }

        /// <summary> Проверяет является ли переданный объект указанным типом. </summary>
        /// <param name="obj"> Проверяемый объект. </param>
        /// <param name="type"> Тип для проверки. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void IsInstanceOfType(object obj, Type type)
        {
#if ENABLE_WINMD_SUPPORT
            if (!(obj.GetType().GetTypeInfo().IsSubclassOf(type) || obj.GetType() == type))
#else 
            if (!type.IsInstanceOfType(obj))
#endif
            {
                throw new AssertFailedException($"IsInstanceOfType ({obj.GetType().Name}, {type.Name})");
            }
        }

        /// <summary> Проверяет, что переданный объект не является указанным типом. </summary>
        /// <param name="obj"> Проверяемый объект. </param>
        /// <param name="type"> Тип для проверки. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void IsNotInstanceOfType(object obj, Type type)
        {
#if ENABLE_WINMD_SUPPORT
            if (obj.GetType().GetTypeInfo().IsSubclassOf(type) || obj.GetType() == type)
#else 
            if (type.IsInstanceOfType(obj))
#endif
            {
                throw new AssertFailedException($"IsNotInstanceOfType ({obj.GetType().Name}, {type.Name})");
            }
        }

        /// <summary> Проверяет, что переданный объект null </summary>
        /// <param name="obj"> Проверяемый объект. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void IsNull(object obj)
        {
            if (obj != null)
            {
                throw new AssertFailedException($"IsNull ({obj.GetType().Name})");
            }
        }

        /// <summary> Проверяет, что переданный объект не null </summary>
        /// <param name="obj"> Проверяемый объект. </param>
        /// <exception cref="AssertFailedException"> Тест провален. </exception>
        public static void IsNotNull(object obj)
        {
            if (obj == null)
            {
                throw new AssertFailedException($"IsNotNull");
            }
        }

        /// <summary> Проверяет, что переданная функция бросает исключение указанного типа. </summary>
        /// <param name="del"> Проверяемая функция. </param>
        /// <typeparam name="T"> Тип исключения для проверки. </typeparam>
        /// <exception cref="AssertFailedException"></exception>
        public static void ThrowsException<T>(Action del) where T : Exception
        {
            try
            {
                del?.Invoke();
            }
            catch (Exception e)
            {
                if (e is T) return;
                
                throw new AssertFailedException($"ThrowsException ({typeof(T).Name}) ({e.GetType().Name})");
            }

            throw new AssertFailedException($"ThrowsException ({typeof(T).Name}) (null)");
        }
    }

    /// <summary> Исключения бросаемое при проваленом тесте. </summary>
    internal class AssertFailedException : Exception
    {
        internal AssertFailedException(string message)
            : base($"Assert failed: {message}")
        {
        }
    }
}