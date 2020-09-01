// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
//
// using System;
// using System.Collections.Generic;
//
// namespace HoloTest
// {
//     [HoloTestClass]
//     public class UnitTestExample
//     {
//          [HoloTestCase]
//          public static void TestCase()
//          {
//              Assert.IsTrue(false);
//          }
//         
//         [HoloTestCase]
//         public static void TestCase1()
//         {
//             Assert.IsNull("test");
//         }
//         
//         [HoloTestCase]
//         public static void TestCase2()
//         {
//             throw new Exception("Exception");
//         }
//         
//         [HoloTestCase]
//         public static void TestCase3()
//         {
//             var a = new ArgumentOutOfRangeException();
//             Assert.IsInstanceOfType(a, typeof(ArgumentOutOfRangeException));
//             Assert.IsInstanceOfType(a, typeof(Exception));
//             Assert.IsNotInstanceOfType(a, typeof(UnitTestExample));
//         }
//         
//         [HoloTestCase]
//         public static void TestCase4()
//         {
//             var a = new ArgumentOutOfRangeException();
//             Assert.IsNotInstanceOfType(a, typeof(ArgumentOutOfRangeException));
//         }
//         
//         [HoloTestCase]
//         public static void TestCase5()
//         {
//             var a = new ArgumentOutOfRangeException();
//             Assert.IsNotInstanceOfType(a, typeof(Exception));
//         }
//         
//         [HoloTestCase]
//         public static void TestCase6()
//         {
//             var a = new ArgumentOutOfRangeException();
//             Assert.IsInstanceOfType(a, typeof(UnitTestExample));
//         }
//
//          [HoloTestGenerator]
//          public static IEnumerable<Action> TestGenerator()
//          {
//              bool[] data = {true, false, false, true, true, true, false};
//
//              foreach (var b in data)
//              {
//                  yield return delegate { Assert.IsTrue(b); };
//              }
//          }
//     }
// }