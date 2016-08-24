using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

using System.Text.RegularExpressions;

using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Collections;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Macros;
using Global.ZennoExtensions;
using ZennoLab.Emulation;
using System.Diagnostics;


namespace KredixLabs
{
    public class WebInstance
    {
        //Объявление приватных переменных для хранения значений публичных свойств
        private string strLogSeparator = Environment.NewLine;
        private bool blnVerbose = false;
        private string strAllStepsLog = String.Empty;
        private string strLastStepLog = String.Empty;
        private string strLastStepValue = String.Empty;
        private int intLastStepErrCode = 0;
        private string strLastStepErrDesc = "noerror";

        //Объявление публичных свойств
        public string LogSeparator
        {
            //Нужно ли вести подробный лог (уровень Verbose)
            get { return strLogSeparator; }
            set { strLogSeparator = value; }
        }

        public bool Verbose
        {
            //Нужно ли вести подробный лог (уровень Verbose)
            get { return blnVerbose; }
            set { blnVerbose = value; }
        }

        public string AllStepsLog
        {
            //Полный лог (заполняется новыми строками до очистки методом LogFullClear
            get { return strAllStepsLog; }
            protected set { strAllStepsLog = value; }
        }

        public string LastStepLog
        {
            //Лог выполнения последнего шага (заполняется новыми строками до очистки методом LogFullClear
            get { return strLastStepLog; }
            protected set { strLastStepLog = value; }
        }

        public string LastStepValue
        {
            //Значение элемента, полученное при выполнении последнего шага
            get { return strLastStepValue; }
            protected set { strLastStepValue = value; }
        }

        public int LastStepErrCode
        {
            //Код ошибки
            get { return intLastStepErrCode; }
            protected set
            {
                intLastStepErrCode = value;
                ErrorDescriptionFiller(value);
            }
        }

        public string LastStepErrDesc
        {
            //Значение элемента, полученное при выполнении последнего шага
            get { return strLastStepErrDesc; }
            protected set { strLastStepErrDesc = value; }
        }

        //Объявление приватных методов
        private void ClearPrevStep()
        {
            strLastStepLog = String.Empty;
            strLastStepValue = String.Empty;
            intLastStepErrCode = 0;
            strLastStepErrDesc = String.Empty;
        }

        private void ErrorDescriptionFiller(int intErrorCode)
        {
            switch (intErrorCode)
            {
                case 0:
                    strLastStepErrDesc = "noerror";
                    break;
                case 1:
                    strLastStepErrDesc = "noparent";
                    break;
                case 2:
                    strLastStepErrDesc = "noelement";
                    break;
                case 3:
                    strLastStepErrDesc = "novalue";
                    break;
                case 4:
                    strLastStepErrDesc = "noaction";
                    break;
                case 55:
                default:
                    strLastStepErrDesc = "noidea: ";
                    break;
            }
            return;
        }

        private bool ConditionChecker(string strValue, string strTarget, string strEquality)
        {
            bool blnTargetValueIsFound = false;
            switch (strEquality)
            {
                case "equal":
                    if (strValue == strTarget)
                        blnTargetValueIsFound = true;
                    break;
                case "notequal":
                case "change":
                    if (strValue != strTarget)
                        blnTargetValueIsFound = true;
                    break;
                case "contains":
                    if (strValue.Contains(strTarget))
                        blnTargetValueIsFound = true;
                    break;
                case "regexp":
                    if (Regex.Matches(strValue, strTarget).Count > 0)
                        blnTargetValueIsFound = true;
                    break;
                case "more":
                    if (strValue == String.Empty)
                        strValue = "0";
                    if (Convert.ToDouble(strValue) > Convert.ToDouble(strTarget))
                        blnTargetValueIsFound = true;
                    break;
                case "moreequal":
                case "plus":
                    if (strValue == String.Empty)
                        strValue = "0";
                    if (Convert.ToDouble(strValue) >= Convert.ToDouble(strTarget))
                        blnTargetValueIsFound = true;
                    break;
                case "less":
                    if (strValue == String.Empty)
                        strValue = "0";
                    if (Convert.ToDouble(strValue) < Convert.ToDouble(strTarget))
                        blnTargetValueIsFound = true;
                    break;
                case "lessequal":
                case "minus":
                    if (strValue == String.Empty)
                        strValue = "0";
                    if (Convert.ToDouble(strValue) <= Convert.ToDouble(strTarget))
                        blnTargetValueIsFound = true;
                    break;
                case "random":
                    blnTargetValueIsFound = true;
                    break;
                case "startswith":
                    if (strValue.StartsWith(strTarget))
                        blnTargetValueIsFound = true;
                    break;
                case "endswith":
                    if (strValue.EndsWith(strTarget))
                        blnTargetValueIsFound = true;
                    break;
            }
            return blnTargetValueIsFound;
        }

        //Объявление публичных методов
        public void AddInfoToLog(string strInfoForLog)
        {
            if (strAllStepsLog.Length > 0)
                strAllStepsLog += strLogSeparator;
            strAllStepsLog += strInfoForLog;

            if (strLastStepLog.Length > 0)
                strLastStepLog += strLogSeparator;
            strLastStepLog += strInfoForLog;
        }

        public void ClearLogs()
        {
            strAllStepsLog = String.Empty;
            ClearPrevStep();
        }

        public HtmlElement GetElementXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, int[] arrTimers)
        {
            /*
            Функция для получения объекта HtmlElement по заданному XPath с ожиданием появления на странице
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске
            4) strXPath - XPath искомого элемента
            5) intIndex - Порядковый номер искомого элемента
            6) arrTimers - Массив таймеров:
            [0] - Паузе перед началом работы метода;
            [1] - Задержка на каждом шаге;
            [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Объект класса HtmlElement

            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов

            */

            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            string strErrorDescription = String.Empty;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (TargetTab.IsBusy && blnWaitDownloading)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElement elElement = null;

                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;

                        AddInfoToLog(String.Format("ok: GetElementXPath, element: \"{0}\" index: {1}", strXPath, intIndex));

                        return elElement;

                    }
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: GetElementXPath, element: \"{0}\", index: {1}, error code {2}: {3}", strXPath, intIndex, LastStepErrCode, LastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.GetElementXPath(instance.ActiveTab, true, null, "//button", 0, new int[3] {0, 1500, 5000});
            */
        }

        public HtmlElement GetElementAttribute(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
           string strTag, string strAttrName, string strAttrValue, string strAttrEquality, int intIndex, int[] arrTimers)
        {
            /*
            Функция для получения объекта HtmlElement по заданному XPath с ожиданием появления на странице
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске
            4) strXPath - XPath искомого элемента
            5) intIndex - Порядковый номер искомого элемента
            6) arrTimers - Массив таймеров:
            [0] - Паузе перед началом работы метода;
            [1] - Задержка на каждом шаге;
            [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Объект класса HtmlElement
            
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            */

            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            string strErrorDescription = String.Empty;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (TargetTab.IsBusy && blnWaitDownloading)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByAttribute(strTag, strAttrName, strAttrValue, strAttrEquality, intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByAttribute(strTag, strAttrName, strAttrValue, strAttrEquality, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;

                        AddInfoToLog(String.Format("ok: GetElementAttribute, element: {0}.{1} {2} \"{3}\"", strTag, strAttrName, strAttrEquality, strAttrValue));
                        return elElement;
                    }
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: GetElementAttribute, element: {0}.{1} {2} \"{3}\", error code {4}: {5}",
                strTag, strAttrName, strAttrEquality, strAttrValue, intLastStepErrCode, strLastStepErrDesc);

            AddInfoToLog(strErrorDescription);
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.GetElementXPath(instance.ActiveTab, true, null, "//button", 0, new int[3] {0, 1500, 5000});
            */
        }

        public void ClickElementXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, bool blnScrollTo, int[] arrTimers)
        {
            /*
            Функция для клика по элементу, найденному по XPath
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске
            4) strXPath - XPath искомого элемента
            5) intIndex - Порядковый номер искомого элемента
            6) blnScrollTo - Делать ли ScrollIntoView к элементу перед щелчком по нему
            7) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
            
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            string strErrorDescription = String.Empty;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {

                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (TargetTab.IsBusy && blnWaitDownloading)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;

                        if (blnScrollTo)
                            elElement.ScrollIntoView();

                        elElement.Click();
                        AddInfoToLog(String.Format("ok: ClickXPath, element: \"{0}\" index: {1}", strXPath, intIndex));
                        return;
                    }

                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: ClickXPath, element: \"{0}\", index: {1}, error code {2}: {3}", strXPath, intIndex, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.ClickElementXPath(instance.ActiveTab, true, null, "//button", 0, false, new int[3] {0, 1500, 5000});
            */
        }

        public void WaitElementValueXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, string strValAttr, string strTargetValue, string strValEquality, int[] arrTimers)
        {
            /*
            Функция для ожидания установления значения определённого атрибута целевого элемента к искомому, с поиском элемента по XPath
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strXPath - XPath искомого элемента
            5) intIndex - Порядковый номер искомого элемента
            6) strValAttr - Значение для какого атрибута искать
            7) strTargetValue - Целевое значение, которое ищем. Может быть передано в виде массива с разелением "|"
            8) strValEquality - Тип соответствия. Варианты смотреть во внутреннем методе ConditionChecker.
                "equal" - значение равно условию;
                "contains" - значение включает в себя условие;
                "regexp" - условие - это регулярное выражение, которому соответствует значение;
                "more" - значение атрибута должно стать большим, чем условие;
                "less" - значение атрибута должно стать меньшим, чем условие;
            9) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
            
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущее значение (атрибут value) найденного элемента
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            string strCurrentElementValue = String.Empty;
            string strErrorDescription = String.Empty;

            string[] arrValues = strTargetValue.Split(new Char[] { '|' });

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                DateTime dtStart = DateTime.Now;
                int intQtimeSpent = 0;
                bool blnElementPresent = false;

                if (blnWaitDownloading && TargetTab.IsBusy)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;
                        foreach (string strValueElement in arrValues)
                        {
                            strCurrentElementValue = elElement.GetAttribute(strValAttr).ToString();

                            bool blnTargetValueIsFound = ConditionChecker(strCurrentElementValue, strValueElement, strValEquality);

                            if (blnTargetValueIsFound)
                            {
                                AddInfoToLog(String.Format("ok: WaitXPathElementValue, element: \"{0}\", index: {1}, wait {2}: {3}, now {4}",
                                    strXPath, intIndex, strValEquality, strTargetValue, strCurrentElementValue));
                                LastStepValue = strCurrentElementValue;
                                return;
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementPresent)
                    LastStepErrCode = 3;

            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: WaitXPathElementValue, element: \"{0}\", index: {1}, wait {2}: {3}, now {4}, error code {5}: {6}",
                strXPath, intIndex, strValEquality, strTargetValue, strCurrentElementValue, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            HtmlElement elParentElement = instance.ActiveTab.FindElementByXPath("//div[@class='group_edit']",0);
            wHelper.WaitElementValueXPath(instance.ActiveTab, false, elParentElement, "//div[@id='group_edit_about_addr']", 
                0, "InnerHtml", "Адрес свободен", "equal", new int[3] {0, 1500, 5000});
            */
        }

        public void SelectListValueXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, string strOptionTag, string strValValue, string strValEquality, bool blnScrollTo, int[] arrTimers)
        {
            /*
            Функция для выбора значения списка
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strXPath - XPath искомого элемента
            5) intIndex - Порядковый номер искомого элемента
            6) strOptionTag - Теги, которыми "обёрнуты" варианты для выбора: <li>, <option>...
            7) strValValue - Целевое значение, которое нужно выбрать
            8) strValEquality - Тип соответствия
            9) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущее значение (выбранный элемент) списка
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            string strErrorDescription = String.Empty;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;
            string strCurrentValue = String.Empty;
            string strElementValue = String.Empty;
            bool blnElementPresent = false;
            List<string> lstVariantsValues = new List<string>();

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    HtmlElementCollection colOptions = elElement.FindChildrenByTags(strOptionTag);
                    if (colOptions.Count > 0) //ДОРАБОТАТЬ (кол-во подэлементов - в ошибку)
                    {
                        blnElementPresent = true;

                        if (blnScrollTo)
                            elElement.ScrollIntoView();
                    }

                    int intTargetValueNumber = 0;

                    for (int i = 0; i < colOptions.Count; i++)
                    {
                        HtmlElement elOption = colOptions.GetByNumber(i);
                        strCurrentValue = elOption.GetAttribute("InnerHtml");
                        lstVariantsValues.Add(strCurrentValue);

                        bool blnTargetValueIsFound = ConditionChecker(strCurrentValue, strValValue, strValEquality);

                        if (strOptionTag == "option" && i >= Convert.ToInt32(elElement.GetValue())) //ДОДЕЛАТЬ ОПРЕДЕЛЕНИЕ value для не option-списков    
                            strElementValue = lstVariantsValues[Convert.ToInt32(elElement.GetValue())];

                        if (blnTargetValueIsFound)
                        {
                            if (strOptionTag == "option")
                            {
                                elElement.SetAttribute("value", intTargetValueNumber.ToString());
                            }
                            else
                            {
                                elOption.Click();
                            }

                            strElementValue = lstVariantsValues[Convert.ToInt32(elElement.GetValue())];

                            AddInfoToLog(String.Format("ok: SelectXPathListValue, element: \"{0}\", index: {1}, to select {2}: {3}, now {4}",
                                strXPath, intIndex, strValEquality, strValValue, strElementValue));
                            LastStepValue = strCurrentValue;
                            return;
                        }

                        intTargetValueNumber++;
                    }
                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementPresent)
                    LastStepErrCode = 4;

            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: SelectXPathListValue, element: \"{0}\", index: {1}, to select {2}: {3}, now {4}, error code {5}: {6}",
                strXPath, intIndex, strValEquality, strValValue, strElementValue, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.SelectListValueXPath(instance.ActiveTab, true, null, "//select[@name='domain']", 0, "option", "rambler.ru", "contains", new int[3] {0, 1500, 5000});
            */
        }

        public void WaitElementCountXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intConditionValue, string strConditionEquality, int[] arrTimers)
        {
            /*
            Функция для ожидания присутствия на странице заданного количества элементов, искомых по XPath
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strXpath - путь XPath, по которому ищем экземпляры
            5) intConditionValue - целевое значение атрибута
            6) strConditionEquality - тип соответствия количества элементов условию. Варианты смотреть во внутреннем методе ConditionChecker.
                "equal" - значение равно условию;
                "change" - количество элементов на странице должно измениться (неважно, в какую сторону);
                "more" - значение атрибута должно стать большим, чем условие;
                "less" - значение атрибута должно стать меньшим, чем условие;
                "plus" - значение атрибута должно стать большим, на определённую величину, чем исходное количество;
                "minus" - значение атрибута должно стать меньшим, на определённую величину, чем исходное количество;
            7) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущее количество элементов на странице
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;
            int intStartingElementsCount = -1;
            int intCurrentElementsCount = 0;
            bool blnElementsPresent = false;
            string strErrorDescription = String.Empty;

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (strConditionEquality == "plus" | strConditionEquality == "minus")
                    blnWaitDownloading = false;

                if (blnWaitDownloading && TargetTab.IsBusy)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElementCollection colElements = null;
                    try
                    {
                        colElements = elParent.FindChildrenByXPath(Regex.Replace(strXPath, @"^//", ".//"));
                        blnParentUsed = true;
                    }
                    catch
                    {
                        colElements = TargetTab.FindElementsByXPath(strXPath);
                    }

                    intCurrentElementsCount = colElements.Count;
                    if (intStartingElementsCount == -1)
                        intStartingElementsCount = intCurrentElementsCount;

                    if (intCurrentElementsCount > 0)
                    {
                        blnElementsPresent = true;
                        bool blnTargetCountIsFound = false;

                        switch (strConditionEquality)
                        {
                            case "plus":
                            case "minus":
                                blnTargetCountIsFound = ConditionChecker(intCurrentElementsCount.ToString(),
                                    Convert.ToString(intCurrentElementsCount + intConditionValue), strConditionEquality);
                                break;
                            default:
                                blnTargetCountIsFound = ConditionChecker(intCurrentElementsCount.ToString(),
                                    intConditionValue.ToString(), strConditionEquality);
                                break;
                        }

                        if (blnTargetCountIsFound)
                        {
                            AddInfoToLog(String.Format("ok: WaitElementCountXPath, element: \"{0}\", wait {1}: {2}, now {3}",
                                strXPath, strConditionEquality, intConditionValue, intCurrentElementsCount));

                            strLastStepValue = intCurrentElementsCount.ToString();
                            return;
                        }

                    }
                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementsPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementsPresent)
                    LastStepErrCode = 3;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: WaitElementCountXPath, element: \"{0}\", wait {1}: {2}, now {3}, error code {4}: {5}",
                strXPath, strConditionEquality, intConditionValue, intCurrentElementsCount, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            LastStepValue = intCurrentElementsCount.ToString();
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.WaitElementCountXPath(instance.ActiveTab, true, null, "//div[@class='thumbnail-container']/div/div[@class='preview']", 1, "plus",  new int[3] {0, 1500, 5000});
            */
        }

        public void WaitElementPresenceXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, string strTargetStatus, int[] arrTimers)
        {
            /*
            Функция ожидания появления/исчезновения элемента с заданным XPath
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strXpath - путь XPath, по которому ищем экземпляры
            5) intIndex - индекс целевого элемента при поиске по XPath
            6) strTargetStatus - целевой статус элемента:
                "present" - сработает, когда элемент появится на странице;
                "void" - сработает, когда элемент пропадёт со страницы.
            7) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущий статус ("present" или "void") целевого элемента
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnTargetStatusReached = false;
            string strCurrentStatus = string.Empty;
            string strErrorDescription = String.Empty;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (blnWaitDownloading && TargetTab.IsBusy)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    if (strTargetStatus == "present" && !elElement.IsVoid)
                        blnTargetStatusReached = true;


                    if (strTargetStatus == "void" && elElement.IsVoid)
                        blnTargetStatusReached = true;

                    if (blnTargetStatusReached)
                    {
                        AddInfoToLog(String.Format("ok: WaitElementPresenceXPath, element: \"{0}\", index: {1}, wait: {2}, now {3}",
                            strXPath, intIndex, strTargetStatus, strTargetStatus));

                        strLastStepValue = strTargetStatus;
                        return;
                    }

                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                //Второго кода ошибки быть не может

                if (LastStepErrCode == 0 && !blnTargetStatusReached)
                {
                    LastStepErrCode = 4;
                }

                if (strTargetStatus == "present")
                    strCurrentStatus = "void";

                if (strTargetStatus == "void")
                    strCurrentStatus = "present";
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: WaitElementPresenceXPath, element: \"{0}\", index: {1}, wait: {2}, now {3}, error code {4}: {5}",
                strXPath, intIndex, strTargetStatus, strCurrentStatus, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            LastStepValue = strCurrentStatus;
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.WaitElementPresenceXPath(instance.ActiveTab, true, null, "//input[@id='typeInName']", 0, "present", new int[3] {0, 1500, 5000});
            */
        }

        public void WaitElementStyleXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, string strTargetStyle, bool blnStyleInheritance, int[] arrTimers)
        {
            /*
            Функция ожидания появления/исчезновения элемента с заданным XPath
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strXpath - путь XPath, по которому ищем экземпляры
            5) intIndex - индекс целевого элемента при поиске по XPath
            6) strTargetStyle - целевое значение стиля, например "display:none;", "padding-left: 10px;"
            7) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущее значение атрибута style целевого элемента
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            bool blnTargetStyleReached = false;
            string strCurrentElementStyle = String.Empty;
            string strErrorDescription = String.Empty;

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (blnWaitDownloading && TargetTab.IsBusy)
                    TargetTab.WaitDownloading();

                strTargetStyle = strTargetStyle.Replace(" ", String.Empty).ToLower();
                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;

                        //Проверяем наследованный стиль
                        if (blnStyleInheritance && !elParent.IsVoid)
                        {
                            string strParentElementStyle = elParent.GetAttribute("style").Replace(" ", String.Empty).ToLower();
                            if (strParentElementStyle.Contains(strTargetStyle))
                            {
                                blnTargetStyleReached = true;
                                strCurrentElementStyle = strParentElementStyle;
                            }
                        }
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;
                        strCurrentElementStyle = elElement.GetAttribute("style").Replace(" ", String.Empty).ToLower();
                        if (strCurrentElementStyle.Contains(strTargetStyle))
                            blnTargetStyleReached = true;
                    }

                    if (blnTargetStyleReached)
                    {
                        AddInfoToLog(String.Format("ok: WaitElementStyleXPath, element: \"{0}\", index: {1}, wait: \"{2}\", now {3}",
                            strXPath, intIndex, strTargetStyle, strCurrentElementStyle));

                        strLastStepValue = strCurrentElementStyle;
                        return;
                    }

                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementPresent)
                    LastStepErrCode = 3;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: WaitElementStyleXPath, element: \"{0}\", index: {1}, wait: \"{2}\", now {3}, error code {4}: {5}",
                strXPath, intIndex, strTargetStyle, strCurrentElementStyle, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            strLastStepValue = strCurrentElementStyle;
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.WaitElementStyleXPath(instance.ActiveTab, true, null, "//div[@id='modalLegal']", 0, "display:block", true, new int[3] {0, 1500, 5000});
            */
        }

        public void SetElementValueXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strXPath, int intIndex, string strValue, string strEmulationLevel,
            bool blnUseSelectedItems, bool blnScrollTo, int[] arrTimers)
        {
            /*
            Функция установки значения элемента с заданным XPath
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strXpath - путь XPath, по которому ищем экземпляры
            5) intIndex - индекс целевого элемента при поиске по XPath
            6) strValue - целевое значение элемента
            7) strEmulationLevel - уровень эмуляции при установке значения
            8) blnUseSelectedItems - true if you need use automatic filling standard "select" fields; otherwise false. (читать здесь: https://help.zennolab.com/en/v5/zennoposter/5.5/ZennoLab.CommandCenter~ZennoLab.CommandCenter.HtmlElement~SetValue.html)
            9) blnScrollTo - проматывать ли страницу к элементу перед установкой значения
            10) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущее значение целевого элемента (атрибута "value")
            */

            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            string strErrorDescription = String.Empty;

            string strCurrentElementValue = String.Empty;

            DateTime dtStart = DateTime.Now;
            if (blnVerbose) AddInfoToLog(String.Format("{0}: SetXPathElementValue for XPath {1}", dtStart.ToString("HH:mm:ss"), strXPath)); //Verbose
            int intQtimeSpent = 0;

            try
            {
                if (intPauseBefore > 0)
                {
                    System.Threading.Thread.Sleep(intPauseBefore);
                    if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: ok sleep before", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                }

                if (blnWaitDownloading && TargetTab.IsBusy)
                {
                    TargetTab.WaitDownloading();
                    if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: ok wait tab downloading", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                }

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByXPath(Regex.Replace(strXPath, @"^//", ".//"), intIndex);
                        blnParentUsed = true;
                        if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: waiting element with parent...", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                    }
                    catch
                    {
                        if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: waiting element without parent...", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                        elElement = TargetTab.FindElementByXPath(strXPath, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;
                        strCurrentElementValue = elElement.GetValue();
                        if (strCurrentElementValue != strValue) //Для полей в сервисах, автоматически засирающих введённый текст html-разметкой.
                            strCurrentElementValue = Regex.Replace(elElement.GetValue(), @"<.*?>", String.Empty);

                        if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: element is found, current value is {1}", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds, strCurrentElementValue)); //Verbose

                        if (strCurrentElementValue != strValue)
                        {
                            if (blnScrollTo)
                            {
                                elElement.ScrollIntoView();
                                if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: scrolled into view", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                            }

                            elElement.SetValue(strValue, strEmulationLevel, blnUseSelectedItems);
                            if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: SetValue started", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                        }

                        //Проверяем корректность ввода
                        strCurrentElementValue = elElement.GetAttribute("Value");
                        if (strCurrentElementValue != strValue) //Для полей в сервисах, автоматически засирающих введённый текст html разметкой.
                            strCurrentElementValue = Regex.Replace(elElement.GetValue(), @"<.*?>", String.Empty);

                        if (strCurrentElementValue == strValue)
                        {
                            AddInfoToLog(String.Format("ok: SetElementValueXPath, element: \"{0}\", index: {1}, set to: {2}, now {3}",
                                strXPath, intIndex, strValue, strCurrentElementValue));

                            strLastStepValue = strCurrentElementValue;
                            return;
                        }
                    }

                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                    if (blnVerbose) AddInfoToLog(String.Format("+{0} sec: next step", new TimeSpan(DateTime.Now.Ticks - dtStart.Ticks).TotalSeconds)); //Verbose
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementPresent)
                    LastStepErrCode = 4;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: SetElementValueXPath, element: \"{0}\", index: {1}, set to: {2}, now {3}, error code {4}: {5}",
                strXPath, intIndex, strValue, strCurrentElementValue, intLastStepErrCode, strLastStepErrDesc);
            AddInfoToLog(strErrorDescription);
            strLastStepValue = strCurrentElementValue;
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.SetElementValueXPath(instance.ActiveTab, true, null, "//input[@id='typeInName']", 0, "Василий Петрович", instance.EmulationLevel, false, false, new int[3] {0, 1500, 5000});
            */
        }

        public void ClickRadioButtonXPath(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strVariantsXPath, string strConditionXPath, string strConditionAttribute, string strConditionValue,
            string strConditionEquality, string strClickXPath, bool blnScrollTo, int[] arrTimers)
        {
            /*
            Функция для щелчка по заданному RadionButton в сложных динамических списках вариантов
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент (для упрощения поиска вариантов)
            4) intParentIndex - индекс родительского элемента
            5) blnScrollTo - прокручивать ли содержимое браузера до родительского элемента перед началом работы
            6) strVariantsXPath - XPath отдельного варианта выбора (сам radiobutton+описание, для формирование семейства)
            7) strConditionXPath - XPath описания, по которому будет производиться поиск
            8) strConditionAttribute - Атрибут описания, по которому будет производиться поиск
            9) strConditionValue - Целевое значение атрибута. Можно передавать массив значений, разделённых "|".
            10) strConditionEquality - Тип соответствия значения атрибута целевому. Варианты смотреть во внутреннем методе ConditionChecker.
            11) strClickXPath - XPath элемента, ко которому необходимо кликнуть (index этого элемента и вычисляется соответствием).
            12) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает значение описания, по элементу рядом с которым мы щёлкнули
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            int intQVariantsFound = 0;
            int intTargetVariantNumber = 0;
            string strCurrentVariantValue = String.Empty;
            string strErrorDescription = String.Empty;

            string[] arrValues = strConditionValue.Split(new Char[] { '|' });

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {
                if (blnWaitDownloading && TargetTab.IsBusy)
                    TargetTab.WaitDownloading();

                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                HtmlElementCollection colVariants = null;

                do
                {
                    bool blnSkipStep = false;

                    if (!blnSkipStep)
                    {
                        //Ждём появления вариантов
                        try
                        {
                            colVariants = elParent.FindChildrenByXPath(Regex.Replace(strVariantsXPath, @"^//", ".//"));
                            blnParentUsed = true;
                            if (blnScrollTo)
                                elParent.ScrollIntoView();
                        }
                        catch
                        {
                            colVariants = TargetTab.FindElementsByXPath(strVariantsXPath);
                        }
                        if (colVariants.Count == 0)
                            blnSkipStep = true;
                    }

                    if (!blnSkipStep)
                    {
                        intQVariantsFound = colVariants.Count;
                        if (intQVariantsFound > 0)
                            blnElementPresent = true;

                        //находим нужный вариант и выбираем
                        for (int i = 0; i < colVariants.Count; i++)
                        {
                            HtmlElement elVariant = colVariants.GetByNumber(i);

                            HtmlElement elCondition = elVariant.FindChildByXPath(Regex.Replace(strConditionXPath, @"^//", ".//"), 0);
                            HtmlElement elClick = elVariant.FindChildByXPath(Regex.Replace(strClickXPath, @"^//", ".//"), 0);
                            strCurrentVariantValue = elCondition.GetAttribute(strConditionAttribute).ToString();

                            foreach (string strValueElement in arrValues)
                            {
                                bool blnTargetValueIsFound = ConditionChecker(strCurrentVariantValue, strConditionValue, strConditionEquality);

                                if (blnTargetValueIsFound)
                                {
                                    intTargetVariantNumber = i;

                                    //Элемент найден, кликаем
                                    if (blnScrollTo) //было if (blnScrollTo && elParent.IsVoid), забыл зачем
                                        elClick.ScrollIntoView();

                                    elClick.Click();
                                    AddInfoToLog(String.Format("ok: ClickRadioButtonXPath, variants: \"{0}\", target is {1}: {2}, now: \"{3}\", found: {4} from {5}",
                                        strVariantsXPath, strConditionEquality, strConditionValue, strCurrentVariantValue,
                                        intTargetVariantNumber + 1, intQVariantsFound));

                                    strLastStepValue = strCurrentVariantValue;
                                    return;
                                }
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementPresent)
                    LastStepErrCode = 4;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: ClickRadioButtonXPath, variants: \"{0}\", target is {1}: {2}, now: \"{3}\", found: {4} from {5}, error code {6}: {7}",
                strVariantsXPath, strConditionEquality, strConditionValue, strCurrentVariantValue,
                intTargetVariantNumber + 1, intQVariantsFound, intLastStepErrCode, strLastStepErrDesc);

            AddInfoToLog(strErrorDescription);
            strLastStepValue = strCurrentVariantValue;
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.ClickRadioButtonXPath (instance.ActiveTab, false, "//div[@class='groups_create_box']", 0, "//div[@class='groups_create_section clear_fix']", 
             "//div", "InnerHtml", "Группа", "contains", "//div/div", false, new int[3] {0, 1500, 5000});
            */

        }

        public void WaitElementAttributeAttr(Tab TargetTab, bool blnWaitDownloading, HtmlElement elParent,
            string strTag, string strAttrName, string strAttrValue, string strAttrEquality, int intIndex,
            string strValAttr, string strValValue, string strValEquality, int[] arrTimers)
        {
            /*
            Функция для ожидания установления значения определённого атрибута целевого элемента к искомому, с поиском элемента по значению заданного атрибута
            Входные данные: 
            1) TargetTab - целевая вкладка
            2) blnWaitDownloading - ждать ли окончания загрузки целевой вкладки
            3) elParent - родительский элемент при поиске целевого элемента
            4) strTag - тег элемента, по атрибуту которого будем искать
            5) strAttrName - имя атрибута, по которому ищем
            6) strAttrValue - целевое значение атрибута
            7) strAttrEquality тип соответствия значения атрибута и целевого значения
                возможные значения - как в аргументах функции FindElementByAttribute
            8) intIndex - Порядковый номер искомого элемента
            9) strValAttr - Название целевого атрибута, установления значения которого будем ждать
            10) strValValue - Целевое значение, которое нужно выбрать
            11) strValEquality - Тип соответствия. Варианты смотреть во внутреннем методе ConditionChecker.
            12) arrTimers - Массив таймеров:
                [0] - Паузе перед началом работы метода;
                [1] - Задержка на каждом шаге;
                [2] - Общий таймаут, при достижении которого будет выброшена ошибка;

            Выходные данные:
            1) Метод не возвращает значений
                        
            Изменяемые свойства:
            1) LastStepLog, AllStepsLog - логи последнего шага, всех шагов
            2) LastStepValue - устанавливает текущее значение атрибута, по которому мы выполняли поиск
            */
            ClearPrevStep();

            int intPauseBefore = arrTimers[0]; //Пауза перед началом работы
            int intDelay = arrTimers[1]; //Задержка на каждом шаге
            int intTimeOut = arrTimers[2]; //Таймаут
            bool blnParentUsed = false;
            bool blnElementPresent = false;
            string strErrorDescription = String.Empty;
            string strCurrentElementValue = String.Empty;

            string[] arrValues = strValValue.Split(new Char[] { '|' });

            DateTime dtStart = DateTime.Now;
            int intQtimeSpent = 0;

            try
            {
                if (intPauseBefore > 0)
                    System.Threading.Thread.Sleep(intPauseBefore);

                if (blnWaitDownloading && TargetTab.IsBusy)
                    TargetTab.WaitDownloading();

                do
                {
                    HtmlElement elElement = null;
                    try
                    {
                        elElement = elParent.FindChildByAttribute(strTag, strAttrName, strAttrValue, strAttrEquality, intIndex);
                        blnParentUsed = true;
                    }
                    catch
                    {
                        elElement = TargetTab.FindElementByAttribute(strTag, strAttrName, strAttrValue, strAttrEquality, intIndex);
                    }

                    if (!elElement.IsVoid)
                    {
                        blnElementPresent = true;
                        foreach (string strValueElement in arrValues)
                        {
                            strCurrentElementValue = elElement.GetAttribute(strValAttr).ToString();

                            bool blnTargetValueIsFound = ConditionChecker(strCurrentElementValue, strValueElement, strValEquality);

                            if (blnTargetValueIsFound)
                            {
                                AddInfoToLog(String.Format("ok: WaitElementAttributeAttr, element: {0}.{1} {2} \"{3}\", wait {4}: {5}, now {6}",
                                    strTag, strAttrName, strAttrEquality, strAttrValue, strValEquality, strValValue, strCurrentElementValue));

                                strLastStepValue = strCurrentElementValue;
                                return;
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(intDelay);
                    TimeSpan difftime = DateTime.Now - dtStart;
                    intQtimeSpent = Convert.ToInt32(Math.Floor(difftime.TotalMilliseconds));
                } while (intQtimeSpent < intTimeOut);

                if (blnParentUsed && elParent.IsVoid)
                    LastStepErrCode = 1;

                if (LastStepErrCode == 0 && !blnElementPresent)
                    LastStepErrCode = 2;

                if (LastStepErrCode == 0 && blnElementPresent)
                    LastStepErrCode = 4;
            }
            catch (Exception e)
            {
                LastStepErrCode = 55;
                LastStepErrDesc += e.Message;
            }

            strErrorDescription = String.Format("err: WaitElementAttributeAttr, element: {0}.{1} {2} \"{3}\", wait {4}: {5}, now {6}, error code {7}: {8}",
                strTag, strAttrName, strAttrEquality, strAttrValue, strValEquality, strValValue, strCurrentElementValue, intLastStepErrCode, strLastStepErrDesc);

            AddInfoToLog(strErrorDescription);
            strLastStepValue = strCurrentElementValue;
            throw new System.Exception(strErrorDescription);

            /*Проверочный вызов:
            SibboraHelper.WebHelper wHelper = new SibboraHelper.WebHelper();
            wHelper.WaitElementAttributeAttr(instance.Activetab, true, null, "span", "class", "h1 bold black", "regexp", 0, "InnerHtml", "Регистрация|Sign Up", "contains", new int[3] {0, 1500, 5000});
            */
        }

    }

        public class Counters
        {
            public int albums { get; set; }
            public int videos { get; set; }
            public int audios { get; set; }
            public int notes { get; set; }
            public int photos { get; set; }
            public int groups { get; set; }
            public int gifts { get; set; }
            public int friends { get; set; }
            public int online_friends { get; set; }
            public int mutual_friends { get; set; }
            public int user_photos { get; set; }
            public int user_videos { get; set; }
            public int followers { get; set; }
            public int subscriptions { get; set; }
            public int pages { get; set; }
        }

        public class Response
        {
            public int uid { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string maiden_name { get; set; }
            public string status { get; set; }
            public Counters counters { get; set; }
            public string interests { get; set; }
            public string movies { get; set; }
            public string tv { get; set; }
            public string books { get; set; }
            public string about { get; set; }
            public string quotes { get; set; }
            public int pid { get; set; }
            public int aid { get; set; }
            public int owner_id { get; set; }
            public string src { get; set; }
            public string src_big { get; set; }
            public string src_small { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string text { get; set; }
            public int created { get; set; }
            public int post_id { get; set; }
            public string src_xbig { get; set; }
            public double? lat { get; set; }
            public double? @long { get; set; }
            public string src_xxbig { get; set; }
            public string src_xxxbig { get; set; }
            public int id { get; set; }
    }

        public class RootObject
        {
            public List<Response> response { get; set; }
        }

}
