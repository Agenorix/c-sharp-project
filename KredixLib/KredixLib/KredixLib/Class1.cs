using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Collections;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;
using ZennoLab.Macros;
using Global.ZennoExtensions;
using ZennoLab.Emulation;
using System.Web;

namespace KredixLib
{
    public class kredixlib
    {
        //Объявляем общие переменные
        //Ключи API
        private string ApiKey_smsreg = string.Empty;
        private string ApiKey_smsactivate = string.Empty;
        private string ApiKey_simsms = string.Empty;
        private string ApiKey_smsvk = string.Empty;
        private string ApiKey_smsarea = string.Empty;
        private string ApiKey_onlinesim = string.Empty;

        //Сервисы активации
        private string smsactivate = string.Empty;
        private string smsreg = string.Empty;
        private string simsms = string.Empty;
        private string smsvk = string.Empty;
        private string smsarea = string.Empty;
        private string onlinesim = string.Empty;

        //Общие переменные
        private string Proxy = string.Empty;
        private string Service = string.Empty;
        //private string Forward = string.Empty;
        private string Operator = string.Empty;
        private string Number = string.Empty;
        private string Id = string.Empty;
        private string smsvk_balance = string.Empty;

        //Списки
        List<string> sms_services = new List<string>();

        public string getNumber (string Services_Activate, string Service_Id, string Operator, string Proxy, string ApiKey_smsreg, string ApiKey_smsactivate, 
            string ApiKey_simsms, string ApiKey_smsvk, string ApiKey_smsarea,
            string ApiKey_onlinesim, out string Number, out string Id)
        {
            Number = string.Empty;
            Id = string.Empty;
            string servicebalance = string.Empty;
            string[] arrServices = Services_Activate.Split(','); //Поместили списко сервисов активации в массов

            for (int i=0;i<arrServices.Length; i++)
            {
                string service = arrServices[i];
                switch (service)
                {
                    case "sms-activate.ru":
                        //Получаем баланс sms-activate.ru
                        string smsactivate_getbalance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        string smsactivate_balance = System.Text.RegularExpressions.Regex.Replace(smsactivate_getbalance, @".*?:", "");
                        if (Convert.ToInt32(smsactivate_balance) == 0)
                        {
                            break;
                        }

                        //Запрашиваем в sms-activate.ru количество свободных номеров
                        string getNumbersStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getNumbersStatus", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        var jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                        Dictionary<string, object> data = jsonser.Deserialize<Dictionary<string, object>>(getNumbersStatus);
                        switch (Service_Id)
                        {
                            case "vk":
                                servicebalance = data["vk_0"].ToString();
                                break;

                            case "ok":
                                servicebalance = data["ok_0"].ToString();
                                break;

                            case "wa":
                                servicebalance = data["wa_0"].ToString();
                                break;

                            case "vi":
                                servicebalance = data["vi_0"].ToString();
                                break;

                            case "tg":
                                servicebalance = data["tg_0"].ToString();
                                break;

                            case "wb":
                                servicebalance = data["wb_0"].ToString();
                                break;

                            case "go":
                                servicebalance = data["go_0"].ToString();
                                break;

                            case "av":
                                servicebalance = data["av_0"].ToString();
                                break;

                            case "av_1":
                                servicebalance = data["av_1"].ToString();
                                break;

                            case "fb":
                                servicebalance = data["fb_0"].ToString();
                                break;

                            case "tw":
                                servicebalance = data["tw_0"].ToString();
                                break;

                            case "ub":
                                servicebalance = data["ub_0"].ToString();
                                break;

                            case "qw":
                                servicebalance = data["qw_0"].ToString();
                                break;

                            case "gt":
                                servicebalance = data["gt_0"].ToString();
                                break;

                            case "sn":
                                servicebalance = data["sn_0"].ToString();
                                break;

                            case "ig":
                                servicebalance = data["ig_0"].ToString();
                                break;

                            case "ss":
                                servicebalance = data["ss_0"].ToString();
                                break;

                            case "ym":
                                servicebalance = data["ym_0"].ToString();
                                break;

                            case "ya":
                                servicebalance = data["ya_0"].ToString();
                                break;

                            case "ma":
                                servicebalance = data["ma_0"].ToString();
                                break;

                            case "mm":
                                servicebalance = data["mm_0"].ToString();
                                break;

                            case "uk":
                                servicebalance = data["uk_0"].ToString();
                                break;

                            case "me":
                                servicebalance = data["me_0"].ToString();
                                break;

                            case "mb":
                                servicebalance = data["mb_0"].ToString();
                                break;

                            case "we":
                                servicebalance = data["we_0"].ToString();
                                break;

                            case "ot":
                                servicebalance = data["ot_0"].ToString();
                                break;

                            case "ot_1":
                                servicebalance = data["ot_1"].ToString();
                                break;

                            default:
                                throw new Exception("Введите правильную абревиатуру сервиса");
                        }
                        if (Int32.Parse(servicebalance) == 0)
                        {
                            return "Нулевой баланс в сервисе";
                        }

                        //Получаем номер в сервисе sms-activate.ru
                        string getnumber = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate
                            + "&action=getNumber&service=" + Service_Id + "&operator=" + Operator, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        switch (getnumber)
                        {
                            case "BAD_KEY":
                                throw new Exception("Неверный API-ключ");

                            case "NO_KEY":
                                throw new Exception("Укажите API-ключ");

                            case "ERROR_SQL":
                                throw new Exception("ошибка SQL-сервера");

                            case "WRONG_SERVICE":
                                throw new Exception("Неправильно указан сервис, который нужно активировать");

                            default:
                                //Получаем номер
                                Number = System.Text.RegularExpressions.Regex.Replace(getnumber, @".*:", "");

                                //Получаем id
                                string idtemp = System.Text.RegularExpressions.Regex.Replace(getnumber, @"ACCESS.*?:", "");
                                Id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

                                return "Получили номер и id сервиса sms-activate";
                        }

                        //break;
                }
            }





              //Получаем баланс sms-reg.ru
            string smsreg_getbalance = ZennoPoster.HttpGet("http://api.sms-reg.com/getBalance.php?apikey=" + ApiKey_smsreg, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            var smsreg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> smsreg_data = smsreg_jsonser.Deserialize<Dictionary<string, object>>(smsreg_getbalance);
            string smsreg_response = smsreg_data["response"].ToString();
            string smsreg_balance = smsreg_data["balance"].ToString();

            //Получаем баланс simsms.org
            string simsms_getbalance = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_balance&service=opt4&apikey=" + ApiKey_simsms, Proxy, 
                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getbalance);
            string simsms_response = simsms_data["response"].ToString();
            string simsms_balance = simsms_data["balance"].ToString();

            //Получаем баланс smsvk.net
            //Проверяем статус сим карты
            string smsvk_getSimStatus = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getSimStatus", Proxy,
                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

            //Получаем баланс
            for (int i = 0; i < 10; i++)
            {
                if (smsvk_getSimStatus.Contains("OK"))
                {
                    string smsvk_getbalance = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getBalance", Proxy,
                        "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                    smsvk_balance = System.Text.RegularExpressions.Regex.Replace(smsvk_getbalance, @".*?:", "");
                }
                else
                {
                    System.Threading.Thread.Sleep(60000);
                }
            }

            //Получение баланса smsarea.org
            string smsarea_getbalance = ZennoPoster.HttpGet("http://sms-area.org/stubs/handler_api.php?api_key=" + ApiKey_smsarea + "&action=getBalance", Proxy,
                    "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
            string smsarea_balance = System.Text.RegularExpressions.Regex.Replace(smsarea_getbalance, @".*?:", "");
            sms_services.Add(smsarea_balance);

          return "OK";
        }

    }
}
