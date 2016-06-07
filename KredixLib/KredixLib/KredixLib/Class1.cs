using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
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
        string simsms_countnumber = string.Empty;

        //Списки
        List<string> sms_services = new List<string>();

        public string getNumber (string Services_Activate, string Service_Id, string Operator, string Proxy, string ApiKey_smsreg, string ApiKey_smsactivate, 
            string ApiKey_simsms, string ApiKey_smsvk, string ApiKey_smsarea,
            string ApiKey_onlinesim, string count, out string Number, out string Id)
        {
            Number = string.Empty;
            Id = string.Empty;
            string servicebalance = string.Empty;//Баланс сервисов активации
            string Site_Id = string.Empty;//Id сайтов в разных сервисах активации
            string price = string.Empty;//сумма активации номера
            string smsreg_balance = string.Empty;//Баланс сервиса sms-reg.com
            string tzid = string.Empty;//id сервиса sms-reg.com
            string simsms_service = string.Empty;
            string response_sm = string.Empty;
            string simsms_service_id = string.Empty;

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

                            switch (smsreg_error)
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
                        string getSimStatus = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key=" + ApiKey_smsvk + "&action=getSimStatus",
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

                        //[smsvk.net] Получаем номер и id
                        string smsvk_getnumber = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key="
                            + ApiKey_smsvk + "&action=getNumber&service=" + Site_Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

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
                    case "sms-area.org":

                        //[sms-area.com] Получение баланса
                        string smsarea_getbalance = ZennoPoster.HttpGet("http://sms-area.org/stubs/handler_api.php?api_key=" + ApiKey_smsarea + "&action=getBalance", Proxy,
                                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                        string smsarea_balance = System.Text.RegularExpressions.Regex.Replace(smsarea_getbalance, @".*?:", "");

                        switch (smsarea_getbalance) // Обрабатываем ошибки сервиса
                        {
                            case "BAD_KEY":
                                throw new Exception("Неверный API-ключ");

                            case "NO_KEY":
                                throw new Exception("Укажите API-ключ");

                            case "ERROR_SQL":
                                throw new Exception("ошибка SQL-сервера");

                            case "WRONG_SERVICE":
                                throw new Exception("Неправильно указан сервис, который нужно активировать");

                            case "NO_ACTION":
                                throw new Exception("Не указана API-функция");

                            case "BAD_ACTION":
                                throw new Exception("Неверная API-функция");
                        }

                        //[sms-area.org] Получаем номер
                        switch (Service_Id)
                        {
                            case "вконтакте":
                                Site_Id = "vk";
                                break;

                            case "Мамба":
                                Site_Id = "mb";
                                break;

                            case "одноклассники":
                                Site_Id = "ok";
                                break;

                            case "4game":
                                Site_Id = "4g";
                                break;

                            case "facebook":
                                Site_Id = "fb";
                                break;

                            case "seosprint":
                                Site_Id = "ss";
                                break;

                            case "instagram":
                                Site_Id = "ig";
                                break;

                            case "webtransfer":
                                Site_Id = "wt";
                                break;

                            case "telegram":
                                Site_Id = "tg";
                                break;

                            case "viber":
                                Site_Id = "vr";
                                break;

                            case "whatsupp":
                                Site_Id = "wa";
                                break;

                            case "webmoney":
                                Site_Id = "wm";
                                break;

                            case "qiwi":
                                Site_Id = "qm";
                                break;

                            case "яндекс":
                                Site_Id = "ym";
                                break;

                            case "google":
                                Site_Id = "gm";
                                break;

                            case "ценобой":
                                Site_Id = "cb";
                                break;

                            case "avito":
                                Site_Id = "at";
                                break;

                            default:
                                Site_Id = "or";
                                break;
                        }

                        string smsarea_getnumber = ZennoPoster.HttpGet("http://sms-area.org/stubs/handler_api.php?api_key="
                            + ApiKey_smsarea + "&action=getNumber&country=" + Operator + "&service=" + Site_Id +
                            "&count=" + count, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        switch (smsarea_getnumber)
                        {
                            case "BAD_KEY":
                                throw new Exception("Неверный API-ключ");

                            case "NO_KEY":
                                throw new Exception("Укажите API-ключ");

                            case "ERROR_SQL":
                                throw new Exception("ошибка SQL-сервера");

                            case "WRONG_SERVICE":
                                throw new Exception("Неправильно указан сервис, который нужно активировать");

                            case "NO_ACTION":
                                throw new Exception("Не указана API-функция");

                            case "NO_MEANS":
                                throw new Exception("Недостаточно средств на счету");

                            case "NO_NUMBER":
                                throw new Exception("Нет номеров с заданными параметрами");

                            case "NO_ACTIVATORS":
                                throw new Exception("Неправильно указан сервис, который нужно активировать");

                            case "NO_ACTIVATORS_OVERLOAD":
                                throw new Exception("Все активаторы перегружены");

                            case "NO_ACTIVATORS_RATE":
                                throw new Exception("Ставка активаторов выше вашей");

                            case "BAD_ACTION":
                                throw new Exception("Неверная API-функция");

                            case "BAD_SERVICE":
                                throw new Exception("Неверный сервис");

                            case "BAD_COUNTRY":
                                throw new Exception("Неверная страна");

                            default:
                                //[sms-area.org] Получаем номер
                                Number = System.Text.RegularExpressions.Regex.Replace(smsarea_getnumber, @".*:", "");

                                //[sms-area.org] Получаем id
                                string idtemp = System.Text.RegularExpressions.Regex.Replace(smsarea_getnumber, @"ACCESS.*?:", "");
                                Id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

                                return "Получили номер и id сервиса sms-agea.org";
                        }

                    case "simsms.org":
                        //[SIMSMS.ORG] ПОЛУЧАЕМ НОМЕР

                        //[simsms.org] Составляем список сервисов
                        switch (Service_Id)
                        {
                            case "вконтакте":
                                simsms_service = "opt4";
                                simsms_service_id = "vk";
                                break;

                            case "Мамба":
                                simsms_service = "opt7";
                                simsms_service_id = "mamba";
                                break;

                            case "одноклассники":
                                simsms_service = "opt5";
                                simsms_service_id = "ok";
                                break;

                            case "4game":
                                simsms_service = "opt0";
                                simsms_service_id = "4game";
                                break;

                            case "gmail":
                                simsms_service = "opt1";
                                simsms_service_id = "gmail";
                                break;

                            case "facebook":
                                simsms_service = "opt2";
                                simsms_service_id = "fb";
                                break;

                            case "spacesru":
                                simsms_service = "opt3";
                                simsms_service_id = "spaces";
                                break;

                            case "viber":
                                simsms_service = "opt11";
                                simsms_service_id = "viber";
                                break;

                            case "фотострана":
                                simsms_service = "opt13";
                                simsms_service_id = "fotostrana";
                                break;

                            case "microsoft":
                                simsms_service = "opt15";
                                simsms_service_id = "ms";
                                break;

                            case "instagram":
                                simsms_service = "opt16";
                                simsms_service_id = "instagram";
                                break;

                            case "qiwi":
                                simsms_service = "opt18";
                                simsms_service_id = "qiwi";
                                break;

                            case "whatsapp":
                                simsms_service = "opt20";
                                simsms_service_id = "whatsapp";
                                break;

                            case "webtransfer":
                                simsms_service = "opt21";
                                simsms_service_id = "webtransfer";
                                break;

                            case "seosprint":
                                simsms_service = "opt22";
                                simsms_service_id = "seosprint";
                                break;

                            case "яндекс":
                                simsms_service = "opt23";
                                simsms_service_id = "ya";
                                break;

                            case "webmoney":
                                simsms_service = "opt24";
                                simsms_service_id = "webmoney";
                                break;

                            case "nasimke":
                                simsms_service = "opt25";
                                simsms_service_id = "nasimke";
                                break;

                            case "comnu":
                                simsms_service = "opt26";
                                simsms_service_id = "com";
                                break;

                            case "dodopizzaru":
                                simsms_service = "opt27";
                                simsms_service_id = "dodopizza";
                                break;

                            case "taborru":
                                simsms_service = "opt28";
                                simsms_service_id = "tabor";
                                break;

                            case "telegram":
                                simsms_service = "opt29";
                                simsms_service_id = "telegram";
                                break;

                            case "простоквашино":
                                simsms_service = "opt30";
                                simsms_service_id = "prostock";
                                break;

                            case "другвокруг":
                                simsms_service = "opt31";
                                simsms_service_id = "drugvokrug";
                                break;

                            case "drom":
                                simsms_service = "opt32";
                                simsms_service_id = "drom";
                                break;

                            case "mailru":
                                simsms_service = "opt33";
                                simsms_service_id = "mail";
                                break;

                            case "twitter":
                                simsms_service = "opt41";
                                simsms_service_id = "twitter";
                                break;

                            case "avito":
                                simsms_service = "opt59";
                                simsms_service_id = "avito";
                                break;

                       }

                        //[simsms.org] Получаем баланс
                        string simsms_getbalance = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_balance&service=" + simsms_service +
                            "&apikey=" + ApiKey_simsms, Proxy,
                   "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        System.Threading.Thread.Sleep(3000);

                        switch (simsms_getbalance)
                        {
                            case "API KEY не получен!":
                                throw new Exception("Введен не верный API KEY");

                            case "Недостаточно средств!":
                                throw new Exception("Недостаточно средств для выполнения операции. Пополните Ваш кошелек");

                            case "Превышено количество попыток!":
                                throw new Exception("Задайте больший интервал между вызовами к серверу API");

                            case "Произошла неизвестная ошибка.":
                                throw new Exception("Попробуйте повторить запрос позже");

                            case "Неверный запрос.":
                                throw new Exception("Проверьте синтаксис запроса и список используемых параметров");

                            case "Произошла внутренняя ошибка сервера":
                                throw new Exception("Попробуйте повторить запрос позже.");

                            default:
                                if (simsms_getbalance.Contains("null"))
                                {
                                    var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                    Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getbalance);
                                    string response = simsms_data["response"].ToString();
                                    string number = simsms_data["number"].ToString();
                                    string id = simsms_data["id"].ToString();
                                    string text = simsms_data["text"].ToString();
                                    string extra = simsms_data["extra"].ToString();
                                    string sms = simsms_data["sms"].ToString();

                                    switch (response)
                                    {
                                        case "5":
                                            throw new Exception("Превышено количество запросов в минуту");
                                        case "6":
                                            throw new Exception("Вы забанены на 10 минут, т.к. набрали отрицательную карму");
                                        case "7":
                                            throw new Exception("Превышено количество одновременных потоков. Дождитесь смс от предыдущих заказов");
                                    }

                                }
                                break;

                        }

                        if (simsms_getbalance.Contains("balance"))
                        {
                            var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                            Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getbalance);
                            string simsms_response = simsms_data["response"].ToString();
                            string simsms_balance = simsms_data["balance"].ToString();
                        }

                        if (simsms_getbalance.Contains("error"))
                        {
                            var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                            Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getbalance);
                            string simsms_response = simsms_data["response"].ToString();
                            string simsms_errmes = simsms_data["error_msg"].ToString();
                            throw new Exception(simsms_errmes);
                        }

                        //[simsms.org] //Получаем количество свободных номеров
                        string simsms_numbercount = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_count&service=" + simsms_service +
                            "&apikey=" + ApiKey_simsms + "&service_id=" + simsms_service_id, Proxy,
                   "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        System.Threading.Thread.Sleep(3000);

                        Regex regex = new Regex("(?<={\"response\":\"1\",\"counts ).*(?=\":)");
                        Match match = regex.Match(simsms_numbercount);

                        
                        switch (Convert.ToString(match))
                        {
                            case "Vkontakte":
                                var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_response = simsms_data["response"].ToString();
                                string simsms_countnumber = simsms_data["counts Vkontakte"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Мамба":
                                var simsms_mamba_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_mamba_data = simsms_mamba_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_mamba_response = simsms_mamba_data["response"].ToString();
                                string simsms_mamba_countnumber = simsms_mamba_data["counts Mamba"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Odnoklassniki":
                                var simsms_ok_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ok_data = simsms_ok_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ok_response = simsms_ok_data["response"].ToString();
                                string simsms_ok_countnumber = simsms_ok_data["counts Odnoklassniki"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "4GAME":
                                var simsms_4g_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_4g_data = simsms_4g_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_4g_response = simsms_4g_data["response"].ToString();
                                string simsms_4g_countnumber = simsms_4g_data["counts 4GAME"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Gmail":
                                var simsms_gm_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_gm_data = simsms_gm_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_gm_response = simsms_gm_data["response"].ToString();
                                string simsms_gm_countnumber = simsms_gm_data["counts Gmail"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "FaceBook":
                                var simsms_fb_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_fb_data = simsms_fb_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_fb_response = simsms_fb_data["response"].ToString();
                                string simsms_fb_countnumber = simsms_fb_data["counts FaceBook"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "spaces":
                                var simsms_sp_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_sp_data = simsms_sp_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_sp_response = simsms_sp_data["response"].ToString();
                                string simsms_sp_countnumber = simsms_sp_data["counts spaces"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "":
                                var simsms_vb_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_vb_data = simsms_vb_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_vb_response = simsms_vb_data["response"].ToString();
                                string simsms_vb_countnumber = simsms_vb_data["counts"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Fotostrana":
                                var simsms_fs_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_fs_data = simsms_fs_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_fs_response = simsms_fs_data["response"].ToString();
                                string simsms_fs_countnumber = simsms_fs_data["counts Fotostrana"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "MS":
                                var simsms_ms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ms_data = simsms_ms_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ms_response = simsms_ms_data["response"].ToString();
                                string simsms_ms_countnumber = simsms_ms_data["counts MS"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Instagram":
                                var simsms_ig_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ig_data = simsms_ig_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ig_response = simsms_ig_data["response"].ToString();
                                string simsms_ig_countnumber = simsms_ig_data["counts Instagram"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Qiwi":
                                var simsms_qw_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_qw_data = simsms_qw_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_qw_response = simsms_qw_data["response"].ToString();
                                string simsms_qw_countnumber = simsms_qw_data["counts Qiwi"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Whatsapp":
                                var simsms_wa_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_wa_data = simsms_wa_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_wa_response = simsms_wa_data["response"].ToString();
                                string simsms_wa_countnumber = simsms_wa_data["counts Whatsapp"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "WEBTrasfer":
                                var simsms_wt_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_wt_data = simsms_wt_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_wt_response = simsms_wt_data["response"].ToString();
                                string simsms_wt_countnumber = simsms_wt_data["counts Vkontakte"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "SEOSprint":
                                var simsms_ss_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ss_data = simsms_ss_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ss_response = simsms_ss_data["response"].ToString();
                                string simsms_ss_countnumber = simsms_ss_data["counts SEOSprint"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Yandex":
                                var simsms_ya_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ya_data = simsms_ya_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ya_response = simsms_ya_data["response"].ToString();
                                string simsms_ya_countnumber = simsms_ya_data["counts Yandex"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Webmoney":
                                var simsms_wm_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_wm_data = simsms_wm_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_wm_response = simsms_wm_data["response"].ToString();
                                string simsms_wm_countnumber = simsms_wm_data["counts Webmoney"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Nasimke":
                                var simsms_ns_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ns_data = simsms_ns_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ns_response = simsms_ns_data["response"].ToString();
                                string simsms_ns_countnumber = simsms_ns_data["counts Nasimke"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Com":
                                var simsms_cm_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_cm_data = simsms_cm_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_cm_response = simsms_cm_data["response"].ToString();
                                string simsms_cm_countnumber = simsms_cm_data["counts Com"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Dodopizza":
                                var simsms_dd_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_dd_data = simsms_dd_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_dd_response = simsms_dd_data["response"].ToString();
                                string simsms_dd_countnumber = simsms_dd_data["counts Dodopizza"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Twitter":
                                var simsms_tw_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_tw_data = simsms_tw_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_tw_response = simsms_tw_data["response"].ToString();
                                string simsms_tw_countnumber = simsms_tw_data["counts Twitter"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Avito":
                                var simsms_av_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_av_data = simsms_av_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_av_response = simsms_av_data["response"].ToString();
                                string simsms_av_countnumber = simsms_av_data["counts Avito"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Tabor":
                                var simsms_tb_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_tb_data = simsms_tb_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_tb_response = simsms_tb_data["response"].ToString();
                                string simsms_tb_countnumber = simsms_tb_data["counts Tabor"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Telegram":
                                var simsms_tg_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_tg_data = simsms_tg_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_tg_response = simsms_tg_data["response"].ToString();
                                string simsms_tg_countnumber = simsms_tg_data["counts Telegram"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Prostock":
                                var simsms_pr_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_pr_data = simsms_pr_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_pr_response = simsms_pr_data["response"].ToString();
                                string simsms_pr_countnumber = simsms_pr_data["counts Prostock"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "DrugVokrug":
                                var simsms_dv_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_dv_data = simsms_dv_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_dv_response = simsms_dv_data["response"].ToString();
                                string simsms_dv_countnumber = simsms_dv_data["counts DrugVokrug"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Drom":
                                var simsms_dr_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_dr_data = simsms_dr_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_dr_response = simsms_dr_data["response"].ToString();
                                string simsms_dr_countnumber = simsms_dr_data["counts Drom"].ToString();
                                // return simsms_countnumber;
                                break;

                            case "Mail":
                                var simsms_ml_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                Dictionary<string, object> simsms_ml_data = simsms_ml_jsonser.Deserialize<Dictionary<string, object>>(simsms_numbercount);
                                string simsms_ml_response = simsms_ml_data["response"].ToString();
                                string simsms_ml_countnumber = simsms_ml_data["counts Mail"].ToString();
                                // return simsms_countnumber;
                                break;

                         }

                        //[SIMSMS.ORG] Получаем номер
                        string simsms_getnumber = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_number&service=" + simsms_service +
                            "&apikey=" + ApiKey_simsms + "&country=ru&id=1", Proxy,
                            "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        for (int sims = 0; sims<10; sims++)
                        {
                            Regex regex_response = new Regex("(?<={\"response\":\").*(?=\",\"number\":\")");
                            Match match_response = regex_response.Match(simsms_getnumber);
                            response_sm = Convert.ToString(match_response);

                            switch (response_sm)
                            {
                                case "1":
                                    Regex regex_number = new Regex("(?<=\"number\":\").*(?=\",\"id\":)");
                                    Match match_number = regex_number.Match(simsms_getnumber);
                                    Number = Convert.ToString(match_number);

                                    Regex regex_id = new Regex("(?<=\"id\":).*(?=,\"text\")");
                                    Match match_id = regex_id.Match(simsms_getnumber);
                                    Id = Convert.ToString(match_id);

                                    return "ПОлучили номер в сервисе simsms.org";

                                case "2":
                                    System.Threading.Thread.Sleep(30000);
                                    simsms_getnumber = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_number&service=" + simsms_service +
                                        "&apikey=" + ApiKey_simsms + "&country=ru&id=1", Proxy,
                                       "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                                    break;

                                case "error":
                                    //тут функция обработки ошибок;
                                    break;

                            }

                        }
                        

                        return response_sm;

                       

                        //return simsms_getnumber;
                    default:
                        return "Выберите правильный сервис";
                }
            }

            
          return "OK";
        }

    }
}
