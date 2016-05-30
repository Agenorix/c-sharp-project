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
using System.Web;

namespace sms_activate_lib
{
    public class smsactivate
    {

        private string ApiKey = string.Empty;
        private string Proxy = string.Empty;
        private string Service = string.Empty;
        private string Forward = string.Empty;
        private string Operator = string.Empty;
        private string Number = string.Empty;
        private string Id = string.Empty;

  

        //Приватная функция получения баланса сервиса sms-activate.ru
        private string getbalance(string ApiKey, string Proxy)
        {
           string getbalance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey + 
               "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

            string balance = System.Text.RegularExpressions.Regex.Replace(getbalance, @".*?:", "");
            return balance;
        }

        //Приватная функция получения количества свободных номеров
        private string getNumbersStatus(string ApiKey, string Proxy)
        {
            string getNumbersStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey + 
                "&action=getNumbersStatus", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            var jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> data = jsonser.Deserialize<Dictionary<string, object>>(getNumbersStatus);
            string vk = data["vk_0"].ToString();
            string ok = data["ok_0"].ToString();
            string wa = data["wa_0"].ToString();
            string vi = data["vi_0"].ToString();
            string tg = data["tg_0"].ToString();
            string wb = data["wb_0"].ToString();
            string go = data["go_0"].ToString();
            string av = data["av_0"].ToString();
            string av_1 = data["av_1"].ToString();
            string fb = data["fb_0"].ToString();
            string tw = data["tw_0"].ToString();
            string ub = data["ub_0"].ToString();
            string qw = data["qw_0"].ToString();
            string gt = data["gt_0"].ToString();
            string sn = data["sn_0"].ToString();
            string ig = data["ig_0"].ToString();
            string ss = data["ss_0"].ToString();
            string ym = data["ym_0"].ToString();
            string ya = data["ya_0"].ToString();
            string ma = data["ma_0"].ToString();
            string mm = data["mm_0"].ToString();
            string uk = data["uk_0"].ToString();
            string me = data["me_0"].ToString();
            string mb = data["mb_0"].ToString();
            string we = data["we_0"].ToString();
            string ot = data["ot_0"].ToString();
            string ot_1 = data["ot_1"].ToString();
            return ok;
        }

        //Приватная функция получения номера сервиса snms-activate.ru
        public string smsactivate_getNumber(string ApiKey, string Proxy, string Service, string Forward, string Operator, out string smsactivate_number, out string smsactivate_id)
        {
            smsactivate_number = string.Empty;
            smsactivate_id = string.Empty;

            string getnumber = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey
                + "&action=getNumber&service=" + Service
                + "&forward=" + Forward
                + "&operator=" + Operator
                , Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

            switch (getnumber)
            {
                case "BAD_KEY":
                    throw new Exception("Неверный API-ключ");

                case "NO_KEY":
                    throw new Exception("Укажите API-ключ");

                case "ERROR_SQL":
                    throw new Exception("ошибка SQL-сервера");

                default:
                    //Получаем номер
                    smsactivate_number = System.Text.RegularExpressions.Regex.Replace(getnumber, @".*:", "");

                    //Получаем id
                    string idtemp = System.Text.RegularExpressions.Regex.Replace(getnumber, @"ACCESS.*?:", "");
                    smsactivate_id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

                    return "Получили номер и id сервиса sms-activate";
            }

        }
          
        //Получаем номер сервиса sms-activate.ru
        public string getnumber(string ApiKey, string Proxy, string Service, string Forward, string Operator)
        {
            //Получаем баланс сервиса sms-activate.ru
            string smsactivate_balance = getbalance(ApiKey, Proxy);

                string smsactivate_number = string.Empty;
                string smsactivate_id = string.Empty;
                string smsactivate_numberstatus = getNumbersStatus(ApiKey, Proxy);

                //Получаем номер и id сервиса sms-activate.ru
                string asdasd = smsactivate_getNumber(ApiKey, Proxy, Service, Forward, Operator, out smsactivate_number, out smsactivate_id);

                return asdasd;
        }

        /*Функция получения смс кода
         Входящие значения:
            1) ApiKey - ключ API
            2) Proxy - текущий прокси проекта
            3) Number - номер телефона, на который проводится активация
            4) Id - id активации
            5) ArrTimers - массив таймеров:
               [0] - время ожидания смс
               [1] - общее время ожидания смс 
        */

        private string sms_ok (string ApiKey, string Proxy, string Number, string Id)
        {
            // Объявляем переменные
            string smsstatus = string.Empty;
            string sms = string.Empty;

            string setStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey +
                    "&action=setStatus&status=1&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                 
            switch(setStatus)
            {
                case "ACCESS_READY":
                    for (int i=0; i<16; i++)
                    {
                    smsstatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey +
                                "&action=getStatus&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly); 
                    if (smsstatus.Contains("STATUS_OK"))
                        {
                            sms = System.Text.RegularExpressions.Regex.Replace(smsstatus, @".*OK:", "");
                            continue;
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(60000);
                        }
                    }
                    break;

                case "STATUS_CANCEL":
                    throw new Exception("Истёк срок ожидания прихода смс");

                case "ERROR_SQL":
                    throw new Exception("Ошибка SQL-сервера");

                case "NO_ACTIVATION":
                    throw new Exception("Id активации не существует");

                case "BAD_SERVICE":
                    throw new Exception("Некорректное наименование сервиса");

                case "BAD_STATUS":
                    throw new Exception("Некорректный статус");

                case "BAD_KEY":
                    throw new Exception("Неверный API-ключ");

                case "BAD_ACTION":
                    throw new Exception("Некорректное действие");
            }

            return sms;
        }

        public string getsms(string ApiKey, string Proxy, string Number, string Id)
        {
            string smsko = sms_ok(ApiKey, Proxy, Number, Id);
            return smsko;
        }

        public string finish (string ApiKey, string Proxy, string Id)
        {
            string setFinish = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey +
                    "&action=setStatus&status=6&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            switch (setFinish)
            {
                case "ACCESS_ACTIVATION":
                    return "Завершили регистрацию";
            }
            return "OK";
        }
    }
}
