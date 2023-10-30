using System;
using System.Reflection;
using Vishnu.Interchange;
using System.ComponentModel;

namespace TriggerEventMirrorChecker
{
    /// <summary>
    /// Spiegelt die Ergebnisse eines anderen Vishnu-Knoten.
    /// </summary>
    public class TriggerEventMirrorChecker : INodeChecker
    {
        #region INodeChecker implementation

        /// <summary>
        /// Kann aufgerufen werden, wenn sich der Verarbeitungsfortschritt
        /// des Checkers geändert hat, muss aber zumindest aber einmal zum
        /// Schluss der Verarbeitung aufgerufen werden.
        /// </summary>
        public event ProgressChangedEventHandler? NodeProgressChanged;

        /// <summary>
        /// Rückgabe-Objekt des Checkers
        /// </summary>
        public object? ReturnObject
        {
            get
            {
                return this._returnObject;
            }
            set
            {
                this._returnObject = value;
            }
        }

        /// <summary>
        /// Hier wird der Arbeitsprozess ausgeführt (oder beobachtet).
        /// </summary>
        /// <param name="checkerParameters">Durch Pipe ("|") getrennte Parameterliste:
        /// State[:State(...)]     State kann die Werte 'Null', 'True', 'False' und/oder 'Exception' annehmen.
        /// [|Zustandswechselzeit]  Zeit in Sekunden bis der State wechselt (bei mehreren States), Default 10s.
        /// [|ReturnObject(String), Beispiele: "null:true:false:Exception|20|Resultat" oder "true"||0815".</param>
        /// <param name="treeParameters">Für den gesamten Tree gültige Parameter oder null.</param>
        /// <param name="source">Auslösendes TreeEvent oder null.</param>
        /// <returns>True, False oder null</returns>
        public bool? Run(object? checkerParameters, TreeParameters treeParameters, TreeEvent source)
        {
            this.publish(String.Format($"{source.SourceId}, {source.Logical.ToString()}, {source.SenderId}"));
            this.ReturnObject = null;
            bool? logicalResult = null;
            this.OnNodeProgressChanged(0);
            if (source != null)
            {
                //this.ReturnObject = null;
                this.ReturnObject = logicalResult.ToString() + " (" + source.NodePath + ")";
                if (source.Results != null && source.Results.Count > 0)
                {
                    // 29.08.2023 Nagel+
                    if (source.Results.TryGetValue(source.SourceId, out Result? result)
                        && result != null && result.ReturnObject is Exception)
                    {
                        this.OnNodeProgressChanged(100);
                        throw (Exception)result.ReturnObject;
                    }
                    // 29.08.2023 Nagel-

                    /* 29.08.2023 Nagel+ auskommentiert
                    foreach (Result? result in source.Results.Values)
                    {
                        if (result != null)
                        {
                            //if (this.ReturnObject == null)
                            //{
                            //	this.ReturnObject = result.ReturnObject;
                            //}
                            if (result.ReturnObject is Exception)
                            {
                                this.OnNodeProgressChanged(100);
                                throw (Exception)result.ReturnObject;
                            }
                            //this.ReturnObject = result.ReturnObject;
                        }
                    }
                    */ // 29.08.2023 Nagel-
                }
                //Thread.Sleep(10);
                logicalResult = source.Logical;
            }
            this.OnNodeProgressChanged(100);
            return logicalResult;
        }

        #endregion INodeChecker implementation

        private object? _returnObject;


        private void OnNodeProgressChanged(int progressPercentage)
        {
            NodeProgressChanged?.Invoke(null, new ProgressChangedEventArgs(progressPercentage, null));
        }

        private void publish(string message)
        {
            NetEti.ApplicationControl.InfoController.Say("#" + Assembly.GetExecutingAssembly().GetName() + "#" + ": " + message);
        }
    }
}
