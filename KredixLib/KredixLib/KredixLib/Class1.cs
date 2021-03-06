﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private string ServiceWork = string.Empty;
        private string ApiWork = string.Empty;
        private string smsvk_balance = string.Empty;
        string simsms_countnumber = string.Empty;
        string simsms_service = string.Empty;
        string simsms_service_id = string.Empty;
        string path = @"c:\TestFile.txt";

        //Списки
        List<string> sms_services = new List<string>();

        public string getnumber(string Services_Activate, string Service_Id, string Operator, string Proxy, string ApiKey_smsreg, string ApiKey_smsactivate,
           string ApiKey_simsms, string ApiKey_smsvk, string ApiKey_smsarea,
           string ApiKey_onlinesim, string count, out string Number, out string Id, out string ServiceWork, out string ApiWork)
        {
            Number = string.Empty;
            Id = string.Empty;
            ServiceWork = string.Empty;
            ApiWork = string.Empty;
            string servicebalance = string.Empty;//Баланс сервисов активации
            string Site_Id = string.Empty;//Id сайтов в разных сервисах активации
            string price = string.Empty;//сумма активации номера
            string simsms_service = string.Empty;
            string response_sm = string.Empty;
            string simsms_service_id = string.Empty;
            string[] arrServices = Services_Activate.Split(','); //Поместили списко сервисов активации в массов

            //Создаем файл лога
            FileStream file = new FileStream(path, FileMode.Append);
            StreamWriter fnew = new StreamWriter(file, Encoding.UTF8);
            fnew.WriteLine(DateTime.Now + "  ---> начинаем активацию " + Service_Id);
            fnew.Close();


            for (int i = 0; i < arrServices.Length; i++)
            {
                string service = arrServices[i];
                switch (service)
                {
                    case "sms-activate.ru":
                        //[sms-activate.ru] Получаем баланс
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [sms - activate.ru] - отправляем запрос на получение баланса");
                        fnew.Close();
                        string smsactivate_getbalance = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate +
                            "&action=getBalance", Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        string smsactivate_balance = System.Text.RegularExpressions.Regex.Replace(smsactivate_getbalance, @".*?:", "");
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - баланс" + smsactivate_balance + " рублей");
                        fnew.Close();
                        if (Convert.ToInt32(smsactivate_balance) == 0)
                        {

                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - Денег нет, идем в следующий сервис");
                            fnew.Close();
                            break;
                        }

                        //[sms-activate.ru] Запрашиваем количество свободных номеров
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - запрашиваем количество свободных номеров");
                        fnew.Close();
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

                        if (Int32.Parse(price) > Int32.Parse(servicebalance))
                        {
                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - На балансе недостаточно средств для дальнейшей работы");
                            fnew.Close();
                            return "На балансе недостаточно средств для дальнейшей работы";
                        }


                        //[sms-activate.ru] Получаем номер и id
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - отправляем запрос на получение номера и id");
                        fnew.Close();
                        string getnumber = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiKey_smsactivate
                            + "&action=getNumber&service=" + Site_Id + "&operator=" + Operator, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        switch (getnumber)
                        {
                            case "BAD_KEY":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - неверный API ключ");
                                fnew.Close();
                                throw new Exception("Неверный API-ключ");
                            case "NO_KEY":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - укажите API ключ");
                                fnew.Close();
                                throw new Exception("Укажите API-ключ");
                            case "ERROR_SQL":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - ошибка SQL сервера");
                                fnew.Close();
                                throw new Exception("ошибка SQL-сервера");
                            case "WRONG_SERVICE":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - неправильно указан сервис, который нужно активировать");
                                fnew.Close();
                                throw new Exception("Неправильно указан сервис, который нужно активировать");
                            default:
                                //[sms-activate.ru] Получаем номер
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - получаем номер для активации");
                                fnew.Close();
                                Number = System.Text.RegularExpressions.Regex.Replace(getnumber, @".*:", "");

                                //[sms-activate.ru] Получаем id
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - получаем id активации");
                                fnew.Close();
                                string idtemp = System.Text.RegularExpressions.Regex.Replace(getnumber, @"ACCESS.*?:", "");
                                Id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");

                                ServiceWork = "sms-activate.ru";
                                ApiWork = ApiKey_smsactivate;

                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - номер и id успешно получены");
                                fnew.Close();
                                return "Получили номер и id сервиса sms-activate";
                        }



                    //[SMSVK.NET] ПОЛУЧАЕМ НОМЕР

                    case "smsvk.net":

                        //[smsvk.net] Проверяем баланс
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - проверяем баланс");
                        fnew.Close();
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
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - запрашиваем количество свободных номеров");
                        fnew.Close();
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
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - отправляем запрос на получение номера и id");
                        fnew.Close();
                        string smsvk_getnumber = ZennoPoster.HttpGet("http://smsvk.net/stubs/handler_api.php?api_key="
                            + ApiKey_smsvk + "&action=getNumber&service=" + Site_Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        switch (smsvk_getnumber)
                        {
                            case "BAD_KEY":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - неверный API ключ");
                                fnew.Close();
                                throw new Exception("Неверный API-ключ");

                            case "NO_KEY":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - укажите API ключ");
                                fnew.Close();
                                throw new Exception("Укажите API-ключ");

                            case "ERROR_SQL":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - ошибка SQL сервера");
                                fnew.Close();
                                throw new Exception("ошибка SQL-сервера");

                            case "WRONG_SERVICE":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - неправильно указан сервис, который нужно активировать");
                                fnew.Close();
                                throw new Exception("Неправильно указан сервис, который нужно активировать");

                            default:
                                //[sms-activate.ru] Получаем номер
                                Number = System.Text.RegularExpressions.Regex.Replace(smsvk_getnumber, @".*:", "");
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - получаем номер");
                                fnew.Close();

                                //[sms-activate.ru] Получаем id
                                string idtemp = System.Text.RegularExpressions.Regex.Replace(smsvk_getnumber, @"ACCESS.*?:", "");
                                Id = System.Text.RegularExpressions.Regex.Replace(idtemp, @":7.*", "");
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - получаем id");
                                fnew.Close();

                                ServiceWork = "smsvk.net";
                                ApiWork = ApiKey_smsvk;

                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - номер и id успешно получены");
                                fnew.Close();
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

                                ServiceWork = "sms-area.org";
                                ApiWork = ApiKey_smsarea;

                                return "Получили номер и id сервиса sms-agea.org";
                        }

                    case "simsms.org":
                        //[SIMSMS.ORG] ПОЛУЧАЕМ НОМЕР
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - начинаем получение номера");
                        fnew.Close();
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
                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - начинаем получение баланса");
                        fnew.Close();
                        string simsms_getbalance = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_balance&service=" + simsms_service +
                            "&apikey=" + ApiKey_simsms, Proxy,
                   "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        System.Threading.Thread.Sleep(3000);

                        switch (simsms_getbalance)
                        {
                            case "API KEY не получен!":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - введен неверный API KEY");
                                fnew.Close();
                                throw new Exception("Введен неверный API KEY");
                            case "Недостаточно средств!":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - недостаточно средств для выполнения операции");
                                fnew.Close();
                                throw new Exception("Недостаточно средств для выполнения операции. Пополните Ваш кошелек");
                            case "Превышено количество попыток!":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - зажайте больший интервал между вызовами к серверу API");
                                fnew.Close();
                                throw new Exception("Задайте больший интервал между вызовами к серверу API");
                            case "Произошла неизвестная ошибка.":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - попробуйте повторить запрос позже");
                                fnew.Close();
                                throw new Exception("Попробуйте повторить запрос позже");
                            case "Неверный запрос.":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - проверьте синтаксис запроса и список используемых параметров");
                                fnew.Close();
                                throw new Exception("Проверьте синтаксис запроса и список используемых параметров");
                            case "Произошла внутренняя ошибка сервера":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - произошла внутреняя ошибка сервера");
                                fnew.Close();
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
                                            fnew = new System.IO.StreamWriter(path, true);
                                            fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - превышено количество запросов в минуту");
                                            fnew.Close();
                                            throw new Exception("Превышено количество запросов в минуту");
                                        case "6":
                                            fnew = new System.IO.StreamWriter(path, true);
                                            fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - бан на 10 минут из-за отрицательной кармы");
                                            fnew.Close();
                                            throw new Exception("Вы забанены на 10 минут, т.к. набрали отрицательную карму");
                                        case "7":
                                            fnew = new System.IO.StreamWriter(path, true);
                                            fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - превышено количество одновременных потоков");
                                            fnew.Close();
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
                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - баланс сервиса равен " + simsms_balance);
                            fnew.Close();
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

                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - получаем количество свободных номеров");
                        fnew.Close();
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

                        fnew = new System.IO.StreamWriter(path, true);
                        fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - начинаем получение номера");
                        fnew.Close();
                        string simsms_getnumber = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_number&service=" + simsms_service +
                            "&apikey=" + ApiKey_simsms + "&country=ru&id=1", Proxy,
                            "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                        for (int sims = 0; sims < 10; sims++)
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

                                    ServiceWork = "simsms.org";
                                    ApiWork = ApiKey_simsms;
                                    fnew = new System.IO.StreamWriter(path, true);
                                    fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - номер и id успешно получены");
                                    fnew.Close();
                                    return "ПОлучили номер в сервисе simsms.org";

                                case "2":
                                    fnew = new System.IO.StreamWriter(path, true);
                                    fnew.WriteLine(DateTime.Now + "  ---> [ " + service + " ] - все номера заняты, ждем 30 секунд");
                                    fnew.Close();
                                    System.Threading.Thread.Sleep(30000);
                                    simsms_getnumber = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_number&service=" + simsms_service +
                                        "&apikey=" + ApiKey_simsms + "&country=ru&id=1", Proxy,
                                       "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                                    //break;
                                    continue;

                                case "error":
                                    switch (simsms_getnumber)
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
                                            if (simsms_getnumber.Contains("null"))
                                            {
                                                var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getnumber);
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
                                    break;
                            }

                        }
                        return response_sm;
                    default:
                        return "Выберите правильный сервис";
                }
            }
            return "OK";
        }

        //ФУНКЦИЯ ПОЛУЧЕНИЯ SMS КОДА
        public string getsms(string ServiceWork, string ApiWork, string Proxy, string Id, string Service_Id)
        {
          switch (ServiceWork)
            {
                case "sms-activate.ru":
                    StreamWriter fnew = new System.IO.StreamWriter(path, true);
                    fnew.WriteLine(DateTime.Now + "  ---> [sms - activate.ru] - отправляем запрос на получение смс кода");
                    fnew.Close();
                    string smsstatus = string.Empty;
                    string sms = string.Empty;
                    string setStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiWork +
                    "&action=setStatus&status=1&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                    switch (setStatus)
                    {
                        case "ACCESS_READY":
                            for (int i = 0; i < 16; i++)
                            {
                                smsstatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiWork +
                                            "&action=getStatus&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                                if (smsstatus.Contains("STATUS_OK"))
                                {
                                    sms = System.Text.RegularExpressions.Regex.Replace(smsstatus, @".*OK:", "");
                                    fnew = new System.IO.StreamWriter(path, true);
                                    fnew.WriteLine(DateTime.Now + "  ---> [sms - activate.ru] - код получен");
                                    fnew.Close();
                                    return sms;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(60000);
                                }
                            }
                            break;

                        case "ACCESS_ACTIVATION":
                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> [sms - activate.ru] - номер успешно подтвержден");
                            fnew.Close();
                            return "Номер успешно подтверждён";
                        case "STATUS_CANCEL":
                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> [sms - activate.ru] - истек срок ожидания прихода смс");
                            fnew.Close();
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

                case "smsvk.net":
                    break;

                case "sms-area.org":
                    break;

                case "simsms.org":
                    fnew = new System.IO.StreamWriter(path, true);
                    fnew.WriteLine(DateTime.Now + "  ---> Начинаем получение смс кода " + ServiceWork);
                    fnew.Close();
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
                    fnew = new System.IO.StreamWriter(path, true);
                    fnew.WriteLine(DateTime.Now + "  ---> Отправляем запрос на получение смс кода " + ServiceWork);
                    fnew.Close();
                    string simsms_getsms = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_sms&service=" + simsms_service + "&country=ru&id=" + Id + "&apikey=" + ApiWork, Proxy,
                                 "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                    for (int sims = 0; sims < 10; sims++)
                    {
                        Regex regex_response = new Regex("(?<={\"response\":\").*(?=\",\"number\":\")");
                        Match match_response = regex_response.Match(simsms_getsms);
                        string simsmsresp = Convert.ToString(match_response);

                        switch (simsmsresp)
                        {
                            case "1":
                                Regex regex_simsms_response = new Regex("(?<=\"sms\":\").*(?=\",\"balanceOnPhone)");
                                Match match_simsms_response = regex_simsms_response.Match(simsms_getsms);
                                string simsms_smscode = Convert.ToString(match_simsms_response);
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> Код смс получен " + ServiceWork);
                                fnew.Close();
                                return simsms_smscode;

                            case "2":
                                fnew = new System.IO.StreamWriter(path, true);
                                fnew.WriteLine(DateTime.Now + "  ---> Код в пути, ждем 30 секунд " + ServiceWork);
                                fnew.Close();
                                System.Threading.Thread.Sleep(30000);
                                simsms_getsms = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_sms&service=" + simsms_service + "&country=ru&id=" + Id + "&apikey=" + ApiWork, Proxy,
                                "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                                continue;

                            case "error":
                                switch (simsms_getsms)
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
                                        if (simsms_getsms.Contains("null"))
                                        {
                                            var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                            Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getsms);
                                            string response = simsms_data["response"].ToString();
                                            string number = simsms_data["number"].ToString();
                                            string id = simsms_data["id"].ToString();
                                            string text = simsms_data["text"].ToString();
                                            string extra = simsms_data["extra"].ToString();
                                            string sssms = simsms_data["sms"].ToString();

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
                                break;
                        }
                    }
                    return simsms_getsms;

            }
            return "OK";
        }

        public string getfinishok(string ServiceWork, string ApiWork, string Proxy, string Id)
        {

            switch (ServiceWork)
            {
                case "sms-activate.ru":
                    StreamWriter fnew = new System.IO.StreamWriter(path, true);
                    fnew.WriteLine(DateTime.Now + "  ---> [sms - activate.ru] - завершаем активацию номера");
                    fnew.Close();
                    string smsstatus = string.Empty;
                    string sms = string.Empty;
                    string setStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiWork +
                    "&action=setStatus&status=6&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                    switch (setStatus)
                    {
                        case "ACCESS_READY":
                            for (int i = 0; i < 16; i++)
                            {
                                smsstatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiWork +
                                            "&action=getStatus&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                                if (smsstatus.Contains("STATUS_OK"))
                                {
                                    sms = System.Text.RegularExpressions.Regex.Replace(smsstatus, @".*OK:", "");
                                    return sms;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(60000);
                                }
                            }
                            break;

                        case "ACCESS_ACTIVATION":
                            return "Номер успешно подтверждён";
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

                case "smsvk.net":
                    break;

                case "sms-area.org":
                    break;

            }
            return "OK";
        }

        public string getfinisherror(string ServiceWork, string ApiWork, string Proxy, string Id, string Service_Id)
        {
            switch (ServiceWork)
            {
                case "sms-activate.ru":
                    string smsstatus = string.Empty;
                    string sms = string.Empty;
                    string setStatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiWork +
                    "&action=setStatus&status=8&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                    switch (setStatus)
                    {
                        case "ACCESS_READY":
                            for (int i = 0; i < 16; i++)
                            {
                                smsstatus = ZennoPoster.HttpGet("http://sms-activate.ru/stubs/handler_api.php?api_key=" + ApiWork +
                                            "&action=getStatus&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);
                                if (smsstatus.Contains("STATUS_OK"))
                                {
                                    sms = System.Text.RegularExpressions.Regex.Replace(smsstatus, @".*OK:", "");
                                    return sms;
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(60000);
                                }
                            }
                            break;

                        case "ACCESS_ACTIVATION":
                            return "Номер успешно подтверждён";
                        case "ACCESS_CANCEL":
                            //тут продумать переход к получению нового номера
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

                case "smsvk.net":
                    break;

                case "sms-area.org":
                    break;

                case "simsms.org":
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
                    StreamWriter fnew = new System.IO.StreamWriter(path, true);
                    fnew.WriteLine(DateTime.Now + "  ---> [ " + ServiceWork + " ] Номер уже использован. В бан его :) ");
                    fnew.Close();
                    string simsms_getban = ZennoPoster.HttpGet("http://simsms.org/priemnik.php?metod=get_ban&service=" + simsms_service +
                        "&apikey=" + ApiKey_simsms + "&id=" + Id, Proxy, "UTF-8", ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly);

                    Regex regex_ban = new Regex("(?<={\"response\":\").*(?=\",\"number\":\")");
                    Match match_ban = regex_ban.Match(simsms_getban);
                    string response_ban = Convert.ToString(match_ban);

                    switch (response_ban)
                    {
                        case "1":
                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> Номер успешно забанен в сервисе  " + ServiceWork);
                            fnew.Close();
                            return "Забанили номер в сервисе simsms.org";

                        case "2":
                            fnew = new System.IO.StreamWriter(path, true);
                            fnew.WriteLine(DateTime.Now + "  ---> Какая то ошибка при бане номера в сервисе  " + ServiceWork);
                            fnew.Close();
                            break;

                        case "error":
                            switch (simsms_getban)
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
                                    if (simsms_getban.Contains("null"))
                                    {
                                        var simsms_jsonser = new System.Web.Script.Serialization.JavaScriptSerializer();
                                        Dictionary<string, object> simsms_data = simsms_jsonser.Deserialize<Dictionary<string, object>>(simsms_getban);
                                        string response = simsms_data["response"].ToString();
                                        string number = simsms_data["number"].ToString();
                                        string id = simsms_data["id"].ToString();
                                        string text = simsms_data["text"].ToString();
                                        string extra = simsms_data["extra"].ToString();
                                        string qqsms = simsms_data["sms"].ToString();

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
                            break;
                    }
                    return "OK";
            }

            return "param";
        }

    }
}
