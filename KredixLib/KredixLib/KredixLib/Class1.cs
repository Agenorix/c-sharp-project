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
            string servicebalance = string.Empty;//Баланс сервисов активации
            string Site_Id = string.Empty;//Id сайтов в разных сервисах активации
            string price = string.Empty;//сумма активации номера
            string smsreg_balance = string.Empty;//Баланс сервиса sms-reg.com
            string tzid = string.Empty;//id сервиса sms-reg.com
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
                            case "вконтакте":
                                Site_Id = "vk";
                                servicebalance = data["vk_0"].ToString();
                                price = "9";
                                break;

                            case "одноклассники":
                                Site_Id = "ok";
                                servicebalance = data["ok_0"].ToString();
                                price = "5";
                                break;

                            case "whatsapp":
                                Site_Id = "wa";
                                servicebalance = data["wa_0"].ToString();
                                price = "6";
                                break;

                            case "viber":
                                Site_Id = "vi";
                                servicebalance = data["vi_0"].ToString();
                                price = "3";
                                break;

                            case "telegram":
                                Site_Id = "tg";
                                servicebalance = data["tg_0"].ToString();
                                price = "3";
                                break;

                            case "periscope":
                                Site_Id = "wb";
                                servicebalance = data["wb_0"].ToString();
                                price = "1";
                                break;

                            case "gmail":
                                Site_Id = "go";
                                servicebalance = data["go_0"].ToString();
                                price = "3";
                                break;

                            case "avito":
                                Site_Id = "av";
                                servicebalance = data["av_0"].ToString();
                                price = "4";
                                break;

                            case "avito_1":
                                Site_Id = "av_1";
                                servicebalance = data["av_1"].ToString();
                                price = "30";
                                break;

                            case "facebook":
                                Site_Id = "fb";
                                servicebalance = data["fb_0"].ToString();
                                price = "2";
                                break;

                            case "twitter":
                                Site_Id = "tw";
                                servicebalance = data["tw_0"].ToString();
                                price = "1";
                                break;

                            case "Taxi2412":
                                Site_Id = "ub";
                                servicebalance = data["ub_0"].ToString();
                                price = "2";
                                break;

                            case "qiwi":
                                Site_Id = "qw";
                                servicebalance = data["qw_0"].ToString();
                                price = "6";
                                break;

                            case "gett":
                                Site_Id = "gt";
                                servicebalance = data["gt_0"].ToString();
                                price = "1";
                                break;

                            case "webmoney":
                                Site_Id = "sn";
                                servicebalance = data["sn_0"].ToString();
                                price = "4";
                                break;

                            case "instagram":
                                Site_Id = "ig";
                                servicebalance = data["ig_0"].ToString();
                                price = "5";
                                break;

                            case "seosprint":
                                Site_Id = "ss";
                                servicebalance = data["ss_0"].ToString();
                                price = "2";
                                break;

                            case "alibaba":
                                Site_Id = "ym";
                                servicebalance = data["ym_0"].ToString();
                                price = "2";
                                break;

                            case "яндекс":
                                Site_Id = "ya";
                                servicebalance = data["ya_0"].ToString();
                                price = "1";
                                break;

                            case "taobao":
                                Site_Id = "ma";
                                servicebalance = data["ma_0"].ToString();
                                price = "2";
                                break;

                            case "microsoft":
                                Site_Id = "mm";
                                servicebalance = data["mm_0"].ToString();
                                price = "1";
                                break;

                            case "datingru":
                                Site_Id = "uk";
                                servicebalance = data["uk_0"].ToString();
                                price = "1";
                                break;

                            case "gem4me":
                                Site_Id = "me";
                                servicebalance = data["me_0"].ToString();
                                price = "2";
                                break;

                            case "yahoo":
                                Site_Id = "mb";
                                servicebalance = data["mb_0"].ToString();
                                price = "1";
                                break;

                            case "aol":
                                Site_Id = "we";
                                servicebalance = data["we_0"].ToString();
                                price = "1";
                                break;

                            case "ot_1":
                                servicebalance = data["ot_1"].ToString();
                                price = "30";
                                break;

                            default:
                                Site_Id = "ot";
                                servicebalance = data["ot_0"].ToString();
                                price = "2";
                                break;
                        }
                        
                        if (Int32.Parse(servicebalance) == 0 || Int32.Parse(price) > Int32.Parse(servicebalance))
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


                        if (smsreg_getbalance.Contains("ERROR"))
                        {
                            var smsreg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                            Dictionary<string, object> smsreg_data = smsreg_jsonser.Deserialize<Dictionary<string, object>>(smsreg_getbalance);
                            string smsreg_response = smsreg_data["response"].ToString();
                            string smsreg_error = smsreg_data["error_msg"].ToString();

                            switch(smsreg_error)
                            {
                                case "ERROR_NO_KEY":
                                    throw new Exception("Укажите ApiKey сервиса sms-reg.com. И проверьте сразу ключи остальных сервисов :)");

                                case "ERROR_WRONG_KEY":
                                    throw new Exception("Указан неверный ключ API сервиса sms-reg.com");

                                case "ERROR_KEY_NEED_CHANGE":
                                    throw new Exception("Требует замены ключ API сервиса sms-reg.com");
                            }
                        }
                        else
                        {
                            var smsreg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                            Dictionary<string, object> smsreg_data = smsreg_jsonser.Deserialize<Dictionary<string, object>>(smsreg_getbalance);
                            string smsreg_response = smsreg_data["response"].ToString();
                            smsreg_balance = smsreg_data["balance"].ToString();
                        }

                        //[SMS-REG.COM] ПОЛУЧАЕМ НОМЕР

                        //[sms-reg.com] Создаем операцию на использование номера
                        switch (Service_Id)
                        {
                            case "4game":
                                Site_Id = "4game";
                                price = "3.00";
                                break;

                            case "gmail":
                                Site_Id = "gmail";
                                price = "4.00";
                                break;

                            case "facebook":
                                Site_Id = "facebook";
                                price = "3.00";
                                break;

                            case "mailru":
                                Site_Id = "mailru";
                                price = "3.00";
                                break;

                            case "ВКонтакте":
                                Site_Id = "vk";
                                price = "14.00";
                                break;

                            case "Одноклассники":
                                Site_Id = "classmates";
                                price = "7.00";
                                break;

                            case "twitter":
                                Site_Id = "twitter";
                                price = "3.00";
                                break;

                            case "mamba":
                                Site_Id = "mamba";
                                price = "3.00";
                                break;

                            case "loveplanet":
                                Site_Id = "loveplanet";
                                price = "3.00";
                                break;

                            case "telegram":
                                Site_Id = "telegram";
                                price = "3.00";
                                break;

                            case "badoo":
                                Site_Id = "badoo";
                                price = "3.00";
                                break;

                            case "drugvokrug":
                                Site_Id = "drugvokrug";
                                price = "3.00";
                                break;

                            case "avito":
                                Site_Id = "avito";
                                price = "3.00";
                                break;

                            case "wabos":
                                Site_Id = "wabos";
                                price = "3.00";
                                break;

                            case "steam":
                                Site_Id = "steam";
                                price = "3.00";
                                break;

                            case "fotostrana":
                                Site_Id = "fotostrana";
                                price = "3.00";
                                break;

                            case "hosting":
                                Site_Id = "hosting";
                                price = "3.00";
                                break;

                            case "viber":
                                Site_Id = "Viber";
                                price = "3.00";
                                break;

                            case "whatsapp":
                                Site_Id = "whatsapp";
                                price = "3.00";
                                break;

                            case "tabor":
                                Site_Id = "tabor";
                                price = "3.00";
                                break;

                            case "seosprint":
                                Site_Id = "seosprint";
                                price = "3.00";
                                break;

                            case "instagram":
                                Site_Id = "instagram";
                                price = "3.00";
                                break;

                            case "matroskin":
                                Site_Id = "matroskin";
                                price = "3.00";
                                break;

                            default:
                                Site_Id = "other";
                                price = "4.00";
                                break;
                        }

                        /*
                        if (double.Parse(smsreg_balance) == 0.00 || double.Parse(price) > double.Parse(smsreg_balance))
                            {
                            return "smsreg пуст";
                            }
                            */
                            
                        string getnum = ZennoPoster.HttpGet("http://api.sms-reg.com/getNum.php?country=ru&service=" + Site_Id + "&apikey=" + ApiKey_smsreg, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        if (smsreg_getbalance.Contains("ERROR"))
                        {
                            var smsreg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                            Dictionary<string, object> smsreg_data = smsreg_jsonser.Deserialize<Dictionary<string, object>>(getnum);
                            string smsreg_response = smsreg_data["response"].ToString();
                            string smsreg_error = smsreg_data["error_msg"].ToString();

                            switch (smsreg_error)
                            {
                                case "Service not define":
                                    throw new Exception("В сервисе sms-reg.com не определен сервис");

                                case "WARNING_LOW_BALANCE":
                                    throw new Exception("В сервисе sms-reg.com недостаточно денег на счету");

                                case "Wrong characters in parameters":
                                    throw new Exception("В сервисе sms-reg.com недопустимые символы в передаваемых данных");
                            }
                        }
                        else
                        {
                            var getnum_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                            Dictionary<string, object> getnum_data = getnum_jsonser.Deserialize<Dictionary<string, object>>(getnum);
                            string response = getnum_data["response"].ToString();
                            tzid = getnum_data["tzid"].ToString();
                        }

                        //Получаем номер в сервисе sms-reg.com

                        string getState = ZennoPoster.HttpGet("http://api.sms-reg.com/getState.php?tzid=" + tzid + "&apikey=" + ApiKey_smsreg,
                            Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        for (int gs = 0; gs < 60; gs++)
                        {
                            if (getState.Contains("TZ_NUM_PREPARE"))
                            {
                                var getStatejson = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> getState_data = getStatejson.Deserialize<Dictionary<string, object>>(getState);
                                string response = getState_data["response"].ToString();
                                Number = getState_data["number"].ToString();
                                Site_Id = getState_data["service"].ToString();
                                Id = tzid;
                                return "OK";
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(1000);
                                getState = ZennoPoster.HttpGet("http://api.sms-reg.com/getState.php?tzid=" + tzid + "&apikey=" + ApiKey_smsreg,
                                    Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                            }
                        }
                       break;

                    //[SMSVK.NET] ПОЛУЧАЕМ НОМЕР

                    case "smsvk.net":

                        //[smsvk.net] Проверяем баланс
                        string getSimStatus = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk +"&action=getSimStatus", 
                            Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        for (int qs = 0; qs < 11; qs++)
                        {
                            if (getSimStatus.Contains("OK"))
                            {
                                string smsvk_getbalance = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getBalance", Proxy,
                                    "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                                smsvk_balance = System.Text.RegularExpressions.Regex.Replace(smsvk_getbalance, @".*?:", "");
                            }
                            else
                            {
                                System.Threading.Thread.Sleep(60000);
                                getSimStatus = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getSimStatus",
                                    Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                            }
                        }

                        //[smsvk.net] Запрашиваем количество свободных номеров
                        string smsvkgetNumber = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getNumbersStatus", 
                            Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        var smsvkjsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                        Dictionary<string, object> smsvkdata = smsvkjsonser.Deserialize<Dictionary<string, object>>(smsvkgetNumber);
                        switch (Service_Id)
                        {
                            case "вконтакте":
                                Site_Id = "vk";
                                servicebalance = smsvkdata["vk"].ToString();
                                price = "8";
                                break;

                            case "одноклассники":
                                Site_Id = "ok";
                                servicebalance = smsvkdata["ok"].ToString();
                                price = "5";
                                break;

                            case "whatsapp":
                                Site_Id = "wa";
                                servicebalance = smsvkdata["wa"].ToString();
                                price = "10";
                                break;

                            case "viber":
                                Site_Id = "vb";
                                servicebalance = smsvkdata["vb"].ToString();
                                price = "6";
                                break;

                            case "telegram":
                                Site_Id = "tg";
                                servicebalance = smsvkdata["tg"].ToString();
                                price = "3";
                                break;

                            case "periscope":
                                Site_Id = "ps";
                                servicebalance = smsvkdata["ps"].ToString();
                                price = "1";
                                break;

                            case "gmail":
                                Site_Id = "gg";
                                servicebalance = smsvkdata["gg"].ToString();
                                price = "2";
                                break;

                            case "avito":
                                Site_Id = "av";
                                servicebalance = smsvkdata["av"].ToString();
                                price = "3";
                                break;

                            case "facebook":
                                Site_Id = "fb";
                                servicebalance = smsvkdata["fb"].ToString();
                                price = "2";
                                break;

                            case "twitter":
                                Site_Id = "tw";
                                servicebalance = smsvkdata["tw"].ToString();
                                price = "2";
                                break;

                            case "qiwi":
                                Site_Id = "qw";
                                servicebalance = smsvkdata["qw"].ToString();
                                price = "6";
                                break;

                            case "gett":
                                Site_Id = "gt";
                                servicebalance = smsvkdata["gt"].ToString();
                                price = "1";
                                break;

                            case "instagram":
                                Site_Id = "ig";
                                servicebalance = smsvkdata["ig_0"].ToString();
                                price = "5";
                                break;

                            case "яндекс":
                                Site_Id = "ya";
                                servicebalance = smsvkdata["ya"].ToString();
                                price = "1";
                                break;

                            case "microsoft":
                                Site_Id = "ms";
                                servicebalance = smsvkdata["ms"].ToString();
                                price = "1";
                                break;

                            case "yahoo":
                                Site_Id = "yh";
                                servicebalance = smsvkdata["yh"].ToString();
                                price = "1";
                                break;

                            case "aol":
                                Site_Id = "al";
                                servicebalance = smsvkdata["al"].ToString();
                                price = "1";
                                break;

                            case "Мамба":
                                Site_Id = "ma";
                                servicebalance = smsvkdata["ma"].ToString();
                                price = "4";
                                break;

                            case "4game":
                                Site_Id = "4g";
                                servicebalance = smsvkdata["4g"].ToString();
                                price = "4";
                                break;

                            case "Drom":
                                Site_Id = "dr";
                                servicebalance = smsvkdata["dr"].ToString();
                                price = "1";
                                break;

                            case "avito_1":
                                Site_Id = "rd";
                                servicebalance = smsvkdata["rd"].ToString();
                                price = "20";
                                break;

                            case "mailru":
                                Site_Id = "mb";
                                servicebalance = smsvkdata["mb"].ToString();
                                price = "1";
                                break;

                            case "таксимаксим":
                                Site_Id = "mx";
                                servicebalance = smsvkdata["mx"].ToString();
                                price = "1";
                                break;

                            case "спортмастер":
                                Site_Id = "sm";
                                servicebalance = smsvkdata["sm"].ToString();
                                price = "2";
                                break;

                            case "steam":
                                Site_Id = "st";
                                servicebalance = smsvkdata["st"].ToString();
                                price = "2";
                                break;

                            default:
                                Site_Id = "or";
                                servicebalance = smsvkdata["or"].ToString();
                                price = "2";
                                break;
                        }

                        //[sms-activate.ru] Получаем номер и id
                        string smsvk_getnumber = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" 
                            + ApiKey_smsvk +"&action=getNumber&service=" + Site_Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        switch (smsvk_getnumber)
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
                                Number = System.Text.RegularExpressions.Regex.Replace(smsvk_getnumber, @".*:", "");

                                //[sms-activate.ru] Получаем id
                                string idtemp = System.Text.RegularExpressions.Regex.Replace(smsvk_getnumber, @"ACCESS.*?:", "");
                                Id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

                                return "Получили номер и id сервиса smsvk.net";
                        }

                    //[SMS-AREA.ORG] ПОЛУЧЕНИЕ НОМЕРА
                    case "sms-area.com":

                        //[sms-area.com] Получение баланса
                        string smsarea_getbalance = ZennoPoster.HttpGet("http://sms-area.org/stubs/handler_api.php?api_key=" + ApiKey_smsarea + "&action=getBalance", Proxy,
                                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        string smsarea_balance = System.Text.RegularExpressions.Regex.Replace(smsarea_getbalance, @".*?:", "");
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



          return "OK";
        }

    }
}
