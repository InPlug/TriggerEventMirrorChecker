﻿using System.ComponentModel;
using Vishnu.Interchange;

namespace TriggerEventMirrorCheckerDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TriggerEventMirrorChecker.TriggerEventMirrorChecker triggerEventMirrorChecker = new();
            triggerEventMirrorChecker.NodeProgressChanged += Program.CheckerProgressChanged;

            Console.WriteLine("Ende mit irgendeiner Taste");
            try
            {
                while (true)
                {
                    try
                    {
                        bool? logical = triggerEventMirrorChecker.Run(null, new TreeParameters("MainTree", null),
                          new TreeEvent("TestTreeEvent", "Id0815", "Id4711", "TestTreeEvent", "TestTreeEventPath", true, NodeLogicalState.Done,
                            new ResultDictionary() { { "TestResult", new Result("ResultId", true, NodeState.Finished, NodeLogicalState.Done, "TestReturnObject") } }, null));
                        Console.WriteLine("Checker: {0}, {1}", logical == null ? "null" : logical.ToString(),
                          triggerEventMirrorChecker.ReturnObject == null ? "null" : triggerEventMirrorChecker.ReturnObject.ToString());
                    }
                    catch (ApplicationException ex)
                    {
                        Console.WriteLine("Checker-Exception: {0}", ex.Message);
                    }
                    Thread.Sleep(1000);
                }

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(Environment.NewLine + "beendet.");
            }
        }

        /// <summary>
        /// Wird angesprungen, wenn sich der Verarbeitungsfortschritt des Checkers geändert hat.
        /// </summary>
        /// <param name="sender">Der Checker.</param>
        /// <param name="args">Argumente mit Progress-Fortschritt.</param>
        static void CheckerProgressChanged(object? sender, ProgressChangedEventArgs args)
        {
            Console.WriteLine(args.ProgressPercentage);
            checkBreak();
        }

        static void checkBreak()
        {
            if (Console.KeyAvailable)
            {
                //ConsoleKeyInfo ki = Console.ReadKey();
                //string inKey = ki.KeyChar.ToString().ToUpper();
                //if (inKey == "E")
                //{
                throw new OperationCanceledException("Abbruch!");
                //}
            }
        }
    }
}