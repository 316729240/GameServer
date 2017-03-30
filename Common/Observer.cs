using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GameCommon
{
    /// <summary>
    /// 被观察者
    /// </summary>
    public abstract class Subject
    {
        public List<IObserver> List = new List<IObserver>();
        /// <summary>
        /// 发生改变时,返回符合条件的观察者数据
        /// </summary>
        public List<ObserverReturn> OnChange()
        {
            List<ObserverReturn> returnList = new List<ObserverReturn>();
            for (int i = 0; i < List.Count; i++)
            {
                ObserverReturn r = List[i].Call();
                if (r.RunStatus == ObserverStatus.Yes || r.RunStatus == ObserverStatus.YesAndStop)
                {
                    returnList.Add(r);
                    if (r.RunStatus == ObserverStatus.YesAndStop) i = List.Count;
                }
            }
            return returnList;
        }
        /// <summary>
        /// 添加观察者
        /// </summary>
        /// <param name="t"></param>
        public void AddObserver(IObserver t)
        {
            List.Add(t);
        }/// <summary>
         /// 添加观察者
         /// </summary>
         /// <param name="t"></param>
        public void AddObserver(List<IObserver> t)
        {
            for(int i = 0; i < t.Count; i++)
            {
                List.Add(t[i]);
            }
        }
    }
    /// <summary>
    /// 观察者
    /// </summary>
    public abstract class IObserver
    {
        /// <summary>
        /// 数据支持
        /// </summary>
        public DataProvider DataProvider = null;
        public IObserver(DataProvider data)
        {
            DataProvider = data;
        }
        public abstract ObserverReturn Call();
    }

    /// <summary>
    /// 观察者返回状态
    /// </summary>
    public enum ObserverStatus
    {
        /// <summary>
        /// 不满足条件
        /// </summary>
        Wrong = 0,
        /// <summary>
        /// 满足条件
        /// </summary>
        Yes = 1,
        /// <summary>
        /// 满足并终止
        /// </summary>
        YesAndStop = 2
    }
    /// <summary>
    /// 观察者数据返回结构
    /// </summary>
    public class ObserverReturn
    {
        /// <summary>
        /// 执行状态
        /// </summary>
        public ObserverStatus RunStatus = ObserverStatus.Wrong;//0不满足条件继续执行 1满足条件继续执行 2满足条件终止执行
        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperationType=0;
        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object Data = null;
    }
}
