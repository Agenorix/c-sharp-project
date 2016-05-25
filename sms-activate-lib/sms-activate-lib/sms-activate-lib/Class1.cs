using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Collections;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Macros;
using Global.ZennoExtensions;
using ZennoLab.Emulation;

namespace sms_activate_lib
{
    public class smsactivate
    {

        private string ApiKey = string.Empty;
        private string Proxy = string.Empty;

        //Получаем баланс сервиса sms-activate.ru
        private string getbalance(string ApiKey, string Proxy)
        {
           string Balance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey + 
               "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            string balbal = System.Text.RegularExpressions.Regex.Replace(Balance, @".*?:", "");
            return balbal;
        }

        public string getnumber(string ApiKey, string Proxy)
        {
            //Получаем баланс сервиса sms-activate.ru
            string asd = getbalance(ApiKey, Proxy);
            if (asd.Contains("NO_KEY"))
            {
                return "Не указан ключ api сервиса sms-activate";
            }
            return asd;

        }
    }
}
