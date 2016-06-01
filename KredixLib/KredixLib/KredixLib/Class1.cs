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
            string Site_Id = string.Empty;
            string[] arrServices = Services_Activate.Split(','); //Поместили списко сервисов активации в массов

            for (int i=0;i<arrServices.Length; i++)
            {
                string service = arrServices[i];
                switch (service)
                {
                    case "sms-activate.ru":
                        //[sms-activate.ru] Получаем баланс
                        string smsactivate_getbalance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        string smsactivate_balance = System.Text.RegularExpressions.Regex.Replace(smsactivate_getbalance, @".*?:", "");
                        if (Convert.ToInt32(smsactivate_balance) == 0)
                        {
                            break;
                        }

                        //[sms-activate.ru] Запрашиваем количество свободных номеров
                        string getNumbersStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getNumbersStatus", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        var jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                        Dictionary<string, object> data = jsonser.Deserialize<Dictionary<string, object>>(getNumbersStatus);
                        switch (Service_Id)
                        {
                            case "ВКонтакте":
                                Site_Id = "vk";
                                servicebalance = data["vk_0"].ToString();
                                break;

                            case "Одноклассники":
                                Site_Id = "ok";
                                servicebalance = data["ok_0"].ToString();
                                break;

                            case "whatsapp":
                                Site_Id = "wa";
                                servicebalance = data["wa_0"].ToString();
                                break;

                            case "viber":
                                Site_Id = "vi";
                                servicebalance = data["vi_0"].ToString();
                                break;

                            case "telegram":
                                Site_Id = "tg";
                                servicebalance = data["tg_0"].ToString();
                                break;

                            case "periscope":
                                Site_Id = "wb";
                                servicebalance = data["wb_0"].ToString();
                                break;

                            case "gmail":
                                Site_Id = "go";
                                servicebalance = data["go_0"].ToString();
                                break;

                            case "avito":
                                Site_Id = "av";
                                servicebalance = data["av_0"].ToString();
                                break;

                            case "avito_1":
                                Site_Id = "av_1";
                                servicebalance = data["av_1"].ToString();
                                break;

                            case "facebook":
                                Site_Id = "fb";
                                servicebalance = data["fb_0"].ToString();
                                break;

                            case "twitter":
                                Site_Id = "tw";
                                servicebalance = data["tw_0"].ToString();
                                break;

                            case "Taxi2412":
                                Site_Id = "ub";
                                servicebalance = data["ub_0"].ToString();
                                break;

                            case "qiwi":
                                Site_Id = "qw";
                                servicebalance = data["qw_0"].ToString();
                                break;

                            case "gett":
                                Site_Id = "gt";
                                servicebalance = data["gt_0"].ToString();
                                break;

                            case "webmoney":
                                Site_Id = "sn";
                                servicebalance = data["sn_0"].ToString();
                                break;

                            case "instagram":
                                Site_Id = "ig";
                                servicebalance = data["ig_0"].ToString();
                                break;

                            case "seosprint":
                                Site_Id = "ss";
                                servicebalance = data["ss_0"].ToString();
                                break;

                            case "alibaba":
                                Site_Id = "ym";
                                servicebalance = data["ym_0"].ToString();
                                break;

                            case "яндекс":
                                Site_Id = "ya";
                                servicebalance = data["ya_0"].ToString();
                                break;

                            case "taobao":
                                Site_Id = "ma";
                                servicebalance = data["ma_0"].ToString();
                                break;

                            case "microsoft":
                                Site_Id = "mm";
                                servicebalance = data["mm_0"].ToString();
                                break;

                            case "datingru":
                                Site_Id = "uk";
                                servicebalance = data["uk_0"].ToString();
                                break;

                            case "gem4me":
                                Site_Id = "me";
                                servicebalance = data["me_0"].ToString();
                                break;

                            case "yahoo":
                                Site_Id = "mb";
                                servicebalance = data["mb_0"].ToString();
                                break;

                            case "aol":
                                Site_Id = "we";
                                servicebalance = data["we_0"].ToString();
                                break;

                            case "ot_1":
                                servicebalance = data["ot_1"].ToString();
                                break;

                            default:
                                Site_Id = "ot";
                                servicebalance = data["ot_0"].ToString();
                                break;
                        }
                        if (Int32.Parse(servicebalance) == 0)
                        {
                            return "Нулевой баланс в сервисе";
                        }

                        //[sms-activate.ru] Получаем номер и id
                        string getnumber = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate
                            + "&action=getNumber&service=" + Site_Id + "&operator=" + Operator, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

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
                                //[sms-activate.ru] Получаем номер
                                Number = System.Text.RegularExpressions.Regex.Replace(getnumber, @".*:", "");

                                //[sms-activate.ru] Получаем id
                                string idtemp = System.Text.RegularExpressions.Regex.Replace(getnumber, @"ACCESS.*?:", "");
                                Id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

                                return "Получили номер и id сервиса sms-activate";
                        }

                    case "sms-reg.com":

                        //[sms-reg.com] Получаем баланс
                        string smsreg_getbalance = ZennoPoster.HttpGet("http://api.sms-reg.com/getBalance.php?apikey=" + ApiKey_smsreg, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        var smsreg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                        Dictionary<string, object> smsreg_data = smsreg_jsonser.Deserialize<Dictionary<string, object>>(smsreg_getbalance);
                        string smsreg_response = smsreg_data["response"].ToString();
                        string smsreg_balance = smsreg_data["balance"].ToString();

                        if (Int32.Parse(smsreg_balance) == 0)
                        {
                            return "smsreg пуст";
                        }

                        //[sms-reg.com] Создаем операцию на оспользование номера
                        string getnum = ZennoPoster.HttpGet("", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        break;
                    default:
                        return "Выберите правильный сервис";
                }
            }



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
