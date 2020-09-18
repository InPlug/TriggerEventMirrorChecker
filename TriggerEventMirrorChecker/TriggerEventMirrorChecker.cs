using System;
using System.Reflection;
using Vishnu.Interchange;
using NetEti.Globals;

namespace Vishnu.Demos
{
    /// <summary>
    /// Spiegelt die Ergebnisse eines anderen Vishnu-Knoten.
    /// </summary>
    public class TriggerEventMirrorChecker : INodeChecker
    {
        #region INodeChecker implementation

        /// <summary>
        /// Kann aufgerufen werden, wenn sich der Verarbeitungs-Fortschritt
        /// des Checkers geändert hat, muss aber zumindest aber einmal zum
        /// Schluss der Verarbeitung aufgerufen werden.
        /// </summary>
        public event CommonProgressChangedEventHandler NodeProgressChanged;

        /// <summary>
        /// Rückgabe-Objekt des Checkers
        /// </summary>
        public object ReturnObject
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
        public bool? Run(object checkerParameters, TreeParameters treeParameters, TreeEvent source)
        {
            this.publish(String.Format($"{source.SourceId}, {source.Logical.ToString()}, {source.SenderId}"));
            this.ReturnObject = null;
            bool? logicalResult = null;
            this.OnNodeProgressChanged(this.GetType().Name, 100, 0, ItemsTypes.items);
            if (source != null)
            {
                //this.ReturnObject = null;
                this.ReturnObject = logicalResult.ToString() + " (" + source.NodePath + ")";
                if (source.Results != null && source.Results.Count > 0)
                {
                    foreach (Result result in source.Results.Values)
                    {
                        if (result != null)
                        {
                            //if (this.ReturnObject == null)
                            //{
                            //	this.ReturnObject = result.ReturnObject;
                            //}
                            if (result.ReturnObject is Exception)
                            {
                                this.OnNodeProgressChanged(this.GetType().Name, 100, 100, ItemsTypes.items);
                                throw result.ReturnObject as Exception;
                            }
                            //this.ReturnObject = result.ReturnObject;
                        }
                    }
                }
                //Thread.Sleep(10);
                logicalResult = source.Logical;
            }
            this.OnNodeProgressChanged(this.GetType().Name, 100, 100, ItemsTypes.items);
            return logicalResult;
        }

        #endregion INodeChecker implementation

        private object _returnObject;


        private void OnNodeProgressChanged(string itemsName, int countAll, int countSucceeded, ItemsTypes itemsType)
        {
            if (NodeProgressChanged != null)
            {
                NodeProgressChanged(null, new CommonProgressChangedEventArgs(itemsName, countAll, countSucceeded, itemsType, null));
            }
        }

        private void publish(string message)
        {
            NetEti.ApplicationControl.InfoController.Say("#" + Assembly.GetExecutingAssembly().GetName() + "#" + ": " + message);
        }
    }
}
