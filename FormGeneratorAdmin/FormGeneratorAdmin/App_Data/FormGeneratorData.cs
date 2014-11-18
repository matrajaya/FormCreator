﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Net.Mail;

namespace FormGeneratorAdmin
{
    public class FormGeneratorData
    {
        #region Variables
        private SqlConnection con;

        //mark's Code
        private static string Connection = ConfigurationManager.ConnectionStrings["FormGenerator"].ToString();
        private DataSet Collection = new DataSet();
        String[] paramNames = { "SitecoreID" };

        #endregion

        #region Constructors
        public FormGeneratorData()
        {
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["FormGenerator"].ToString());
        }
        #endregion

        #region Helpers

        private DataSet SQL_SP_Exec(string spName, SqlConnection con, String[] paramNames, Object[] paramValues)
        {
            SqlCommand cmd = new SqlCommand(spName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 14400;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            if (paramNames != null && paramValues != null)
                for (int i = 0; i < paramNames.Length; i++)
                    cmd.Parameters.Add(new SqlParameter(paramNames[i], paramValues[i]));

            try
            {
                con.Open();
                da.Fill(ds);
                con.Close();
            }
            catch(Exception e)
            {
                string[] errorGroup = ConfigurationManager.AppSettings["FormGeneratorErrorGroup"].ToString().Split(',');
                System.Net.Mail.MailMessage errorMail = new System.Net.Mail.MailMessage();

                errorMail.IsBodyHtml = true;
                errorMail.Body = e.ToString();
                errorMail.From = new MailAddress("FGAdminError@esri.com");
                errorMail.Subject = e.Message;

                foreach (string emailMember in errorGroup)
                {
                    errorMail.To.Add(emailMember);
                }

                System.Net.Mail.SmtpClient emailClient = new SmtpClient("SMTP.esri.com", 25);

                try
                {
                    emailClient.Send(errorMail);
                }
                catch (Exception)
                { }            
            }

            return ds;
        }

        #endregion

        #region Data Calls


        public DataTable GetFormControlsByFormID(string FormID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { FormID };

            DataSet ds = SQL_SP_Exec("[spr_GetFormControlsByFormID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable Login(string username, string password)
        {
            String[] paramNames = { "Username", "Password" };
            Object[] paramValues = { username, password };

            DataSet ds = SQL_SP_Exec("[spr_Login]", con, paramNames, paramValues);

            return ds.Tables[0];
        }

        public void LogAction(string admin_ID, string message)
        {
            String[] paramNames = { "Admin_ID", "Message" };
            Object[] paramValues = { admin_ID, message };

            DataSet ds = SQL_SP_Exec("[spr_LogAction]", con, paramNames, paramValues);
        }

        public DataTable AddForm(string formID, string name, string itemID, string trackingCampaign, string trackingSource, string trackingForm, string header, string templateID, string styleID, string aprimoID, string aprimoSubject, string confirmationURL)
        {
            String[] paramNames = { "Form_ID", "Name", "ItemID", "TrackingCampaign", "TrackingSource", "TrackingForm", "Header", "TemplateID", "StyleID", "AprimoID", "AprimoSubject", "ConfirmationURL" };
            Object[] paramValues = { formID, name, itemID, trackingCampaign, trackingSource, trackingForm, header, templateID, styleID, aprimoID, aprimoSubject, confirmationURL };

            DataSet ds = SQL_SP_Exec("[spr_AddForm]", con, paramNames, paramValues);

            return ds.Tables[0];
        }

        public DataTable GetFormByFormID(string FormID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { FormID };

            DataSet ds = SQL_SP_Exec("[spr_GetFormByFormID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetAprimoInfoByForm_ID(string FormID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { FormID };

            DataSet ds = SQL_SP_Exec("[spr_GetAprimoInfoByForm_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetFormBySitecoreItemID(string itemID)
        {
            String[] paramNames = { "ItemID" };
            Object[] paramValues = { itemID };

            DataSet ds = SQL_SP_Exec("[spr_GetFormBySitecoreItemID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataSet GetAllFormDataByFormID(string formID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { formID };

            return SQL_SP_Exec("[spr_GetAllFormDataByFormID]", con, paramNames, paramValues);
        }

        public DataSet GetAllFormDataByFormID_Pagination(string formID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { formID };

            return SQL_SP_Exec("[spr_GetAllFormDataByFormID_Pagination]", con, paramNames, paramValues);
        }

        public DataTable GetTemplateByFormID(string FormID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { FormID };

            DataSet ds = SQL_SP_Exec("[spr_GetTemplateByFormID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetControls()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetControls]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetCustomizableControlTypes()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetCustomizableControlTypes]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetCustomControlFunctions()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetCustomControlFunctions]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable SaveCustomFieldInfo(string FormControl_ID, string CustomLabel, string CustomControlType, string AprimoColumn, string isSpecial, string customControlFunction_ID)
        {
            String[] paramNames = { "FormControl_ID", "CustomLabel", "CustomControlType", "AprimoColumn", "IsSpecial", "CustomControlFunction_ID" };
            Object[] paramValues = { FormControl_ID, CustomLabel, CustomControlType, AprimoColumn, isSpecial, customControlFunction_ID };

            DataSet ds = SQL_SP_Exec("[spr_SaveCustomFieldInfo]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataSet GetCustomFieldInfo(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetCustomFieldInfo]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds;
            else
                return null;
        }

        public DataTable GetAvalableControlsByForm_ID(string FormID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { FormID };

            DataSet ds = SQL_SP_Exec("[spr_GetAvalableControlsByForm_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetControlActionTypes()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetControlActionTypes]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetTemplates()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetTemplates]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public string AddControlToPlaceHolder(string controlList_ID, string FormID, string PlaceholderName, string FormControl_ID, string displayText)
        {
            String[] paramNames = { "ControlList_ID", "FormID", "PlaceholderName", "FormControl_ID", "DisplayText" };
            Object[] paramValues = { controlList_ID, FormID, PlaceholderName, FormControl_ID, displayText };

            DataSet ds = SQL_SP_Exec("[spr_AddControlToPlaceHolder]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return null;
        }

        public string AddControlToPagePlaceHolder(string controlList_ID, string FormID, string PlaceholderName, string FormControl_ID, string displayText, string pageName)
        {
            String[] paramNames = { "ControlList_ID", "FormID", "PlaceholderName", "FormControl_ID", "DisplayText", "PageName" };
            Object[] paramValues = { controlList_ID, FormID, PlaceholderName, FormControl_ID, displayText, pageName };

            DataSet ds = SQL_SP_Exec("[spr_AddControlToPagePlaceHolder]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0].Rows[0][0].ToString();
            else
                return null;
        }

        public int SaveFormControlSetting(object FormControl_ID, string ControlProperty_ID, string settingValue)
        {
            String[] paramNames = { "FormControl_ID", "ControlProperty_ID", "SettingValue"};
            Object[] paramValues = { FormControl_ID, ControlProperty_ID, settingValue };

            DataSet ds = SQL_SP_Exec("[spr_SaveFormControlSetting]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            else
                return 0;
        }

        public int SaveCustomGroupInfo(string formControl_ID, string customLabel, string aprimoColumn)
        {
            String[] paramNames = { "FormControl_ID", "CustomLabel", "AprimoColumn" };
            Object[] paramValues = { formControl_ID, customLabel, aprimoColumn };

            DataSet ds = SQL_SP_Exec("[spr_SaveCustomGroupInfo]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            else
                return 0;
        }

        public DataTable GetCustomGroupInfoByFormControl_ID(string formControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { formControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetCustomGroupInfoByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public string GetControlProperty(object FormControl_ID, string PropertyName)
        {
            String[] paramNames = { "FormControl_ID", "PropertyName" };
            Object[] paramValues = { FormControl_ID, PropertyName };

            DataSet ds = SQL_SP_Exec("[spr_GetControlProperty]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows[0]["SettingValue"].ToString();
                else
                    return "";
            }
            else
                return "";
        }

        public DataTable GetPlaceholdersByTemplateID(string template_ID)
        {
            String[] paramNames = { "Template_ID" };
            Object[] paramValues = { template_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetPlaceholdersByTemplateID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetControlConstantValues(string controlList_ID)
        {
            String[] paramNames = { "ControlList_ID" };
            Object[] paramValues = { controlList_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetControlConstantValues]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public bool GetECASValueByForm_ID(string Form_ID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { Form_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetECASValueByForm_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToBoolean(ds.Tables[0].Rows[0]["ECASOnSubmit"]);
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool DoesFormhaveECASControlAction(string Form_ID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { Form_ID };

            DataSet ds = SQL_SP_Exec("[spr_DoesFormhaveECASControlAction]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToBoolean(ds.Tables[0].Rows[0][0]);
                }
                else
                    return false;
            }
            else
                return false;
        }

        public DataTable GetForms()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetForms]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetStyles()
        {
            DataSet ds = SQL_SP_Exec("[spr_GetStyles]", con, null, null);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetFormControlsByPlaceholderID(string placeholder_ID, string Form_ID)
        {
            String[] paramNames = { "Placeholder_ID", "Form_ID" };
            Object[] paramValues = { placeholder_ID, Form_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetFormControlsByPlaceholderID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public void RemoveFormControl(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_RemoveFormControl]", con, paramNames, paramValues);
        }

        public void RemoveFormByForm_ID(string Form_ID)
        {
            String[] paramNames = { "Form_ID" };
            Object[] paramValues = { Form_ID };

            DataSet ds = SQL_SP_Exec("[spr_RemoveFormByForm_ID]", con, paramNames, paramValues);
        }

        public DataTable GetAllControlActionDataByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetAllControlActionDataByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetAllFormControlGroupItemsByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetAllFormControlGroupItemsByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataSet GetControlActionParametersByControlAction_ID(string controlAction_ID)
        {
            String[] paramNames = { "ControlAction_ID" };
            Object[] paramValues = { controlAction_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetControlActionParametersByControlAction_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds;
            else
                return null;
        }

        public DataTable GetControlActionsByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetControlActionsByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public string GetForm_IDByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetForm_IDByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != null)
                {
                    return ds.Tables[0].Rows[0][0].ToString();
                }
                else
                    return "";
            }
            else
                return "";
        }

        public void GetFormControlByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetFormControlByFormControl_ID]", con, paramNames, paramValues);
        }

        public DataTable AddFormControlGroupItem(string formControl_ID, string text, string value, string formControlGroup_ID)
        {
            String[] paramNames = { "FormControl_ID", "Text", "Value", "FormControlGroup_ID" };
            Object[] paramValues = { formControl_ID, text, value, formControlGroup_ID };

            DataSet ds = SQL_SP_Exec("[spr_AddFormControlGroupItem]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable AddControlAction(string FormControl_ID, string controlActionType_ID)
        {
            String[] paramNames = { "FormControl_ID", "ControlActionType_ID" };
            Object[] paramValues = { FormControl_ID, controlActionType_ID };

            DataSet ds = SQL_SP_Exec("[spr_AddControlAction]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public int SaveMultipleActionParamsByID(string FormControl_ID, string ControlActionType_ID, string DelimData)
        {
            String[] paramNames = { "FormControl_ID", "ControlActionType_ID", "DelimData" };
            Object[] paramValues = { FormControl_ID, ControlActionType_ID, DelimData };

            DataSet ds = SQL_SP_Exec("[spr_SaveMultipleActionParamsByID]", con, paramNames, paramValues);

            return 1;
        }

        public void UpdateRequiredByFormControl_ID(string FormControl_ID, bool isRequired)
        {
            String[] paramNames = { "FormControl_ID", "IsRequired" };
            Object[] paramValues = { FormControl_ID, isRequired };

            DataSet ds = SQL_SP_Exec("[spr_UpdateRequiredByFormControl_ID]", con, paramNames, paramValues);
        }

        public void SetTabOrder(string FormControl_ID, string tabOrder)
        {
            String[] paramNames = { "FormControl_ID", "TabOrder" };
            Object[] paramValues = { FormControl_ID, tabOrder };

            DataSet ds = SQL_SP_Exec("[spr_SetTabOrder]", con, paramNames, paramValues);
        }

        public void SaveSubmitText(string FormControl_ID, string text)
        {
            String[] paramNames = { "FormControl_ID", "Text" };
            Object[] paramValues = { FormControl_ID, text };

            DataSet ds = SQL_SP_Exec("[spr_SaveSubmitText]", con, paramNames, paramValues);
        }

        public void UpdateECAS(string Form_ID, bool value)
        {
            String[] paramNames = { "Form_ID", "Value" };
            Object[] paramValues = { Form_ID, value };

            DataSet ds = SQL_SP_Exec("[spr_UpdateECAS]", con, paramNames, paramValues);
        }

        public void UpdateReturnURLByForm_ID(string Form_ID, string returnURL)
        {
            String[] paramNames = { "Form_ID", "ReturnURL" };
            Object[] paramValues = { Form_ID, returnURL };

            DataSet ds = SQL_SP_Exec("[spr_UpdateReturnURLByForm_ID]", con, paramNames, paramValues);
        }

        public string GetReturnURLByForm_ID(string Form_ID)
        {
            String[] paramNames = { "Form_ID"};
            Object[] paramValues = { Form_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetReturnURLByForm_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["ECASReturnURL"] != null)
                {
                    return ds.Tables[0].Rows[0]["ECASReturnURL"].ToString();
                }
                else
                    return "";
            }
            else
                return "";
        }

        public void UpdateAprimoInfo(string Form_ID, string subject, string aprimoID)
        {
            String[] paramNames = { "Form_ID", "Subject", "ID" };
            Object[] paramValues = { Form_ID, subject, aprimoID };

            DataSet ds = SQL_SP_Exec("[spr_UpdateAprimoInfo]", con, paramNames, paramValues);
        }

        public void SaveFormControlDefaultOption(string defaultOption, string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID", "DefaultOption" };
            Object[] paramValues = { FormControl_ID, defaultOption };

            DataSet ds = SQL_SP_Exec("[spr_SaveFormControlDefaultOption]", con, paramNames, paramValues);
        }

        public void UpdateControlPropertySetting(string FormControl_ID, string propertyName, string value)
        {
            String[] paramNames = { "FormControl_ID", "PropertyName", "Value" };
            Object[] paramValues = { FormControl_ID, propertyName, value };

            DataSet ds = SQL_SP_Exec("[spr_UpdateControlPropertySetting]", con, paramNames, paramValues);
        }

        public DataTable AddControlOption(string FormControl_ID, string text, string value)
        {
            String[] paramNames = { "FormControl_ID", "Text", "Value" };
            Object[] paramValues = { FormControl_ID, text, value };

            DataSet ds = SQL_SP_Exec("[spr_AddControlOption]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable SaveControlOption(string FormControl_ID, string text, string value)
        {
            String[] paramNames = { "FormControl_ID", "Text", "Value" };
            Object[] paramValues = { FormControl_ID, text, value };

            DataSet ds = SQL_SP_Exec("[spr_SaveControlOption]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public void UpdateControlOption(string controlOption_ID, string text, string value)
        {
            String[] paramNames = { "ControlOption_ID", "Text", "Value" };
            Object[] paramValues = { controlOption_ID, text, value };

            DataSet ds = SQL_SP_Exec("[spr_UpdateControlOption]", con, paramNames, paramValues);
        }

        public DataTable GetFormControlPropertyValuesByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetFormControlPropertyValuesByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public string GetDefaultOptionByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetDefaultOptionByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["DefaultValue"] != null)
                {
                    return ds.Tables[0].Rows[0]["DefaultValue"].ToString();
                }
                else
                    return "";
            }
            else
                return "";
        }

        public DataTable GetControlInfoByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetControlInfoByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable RemoveControlAction(string controlAction_ID)
        {
            String[] paramNames = { "ControlAction_ID" };
            Object[] paramValues = { controlAction_ID };

            DataSet ds = SQL_SP_Exec("[spr_RemoveControlAction]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable RemoveFormControlGroupItem(string formControlGroup_ID)
        {
            String[] paramNames = { "FormControlGroup_ID" };
            Object[] paramValues = { formControlGroup_ID };

            DataSet ds = SQL_SP_Exec("[spr_RemoveFormControlGroupItem]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable RemoveElementOption(string controlOption_ID)
        {
            String[] paramNames = { "ControlOption_ID" };
            Object[] paramValues = { controlOption_ID };

            DataSet ds = SQL_SP_Exec("[spr_RemoveControlOption]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public void SetFormControlOrder(string delimitedIDs, string delimiter)
        {
            String[] paramNames = { "DelimitedFormControl_IDs", "Delimiter" };
            Object[] paramValues = { delimitedIDs, delimiter };

            DataSet ds = SQL_SP_Exec("[spr_SetFormControlOrder]", con, paramNames, paramValues);
        }

        public void SetControlOptionOrder(string delimitedIDs, string delimiter)
        {
            String[] paramNames = { "DelimitedControlOption_IDs", "Delimiter" };
            Object[] paramValues = { delimitedIDs, delimiter };

            DataSet ds = SQL_SP_Exec("[spr_SetControlOptionOrder]", con, paramNames, paramValues);
        }

        public DataTable GetControlOptionsByFormControl_ID(string FormControl_ID)
        {
            String[] paramNames = { "FormControl_ID" };
            Object[] paramValues = { FormControl_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetControlOptionsByFormControl_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable GetControlOptionsByControlOption_ID(string controlOption_ID)
        {
            String[] paramNames = { "ControlOption_ID" };
            Object[] paramValues = { controlOption_ID };

            DataSet ds = SQL_SP_Exec("[spr_GetControlOptionsByControlOption_ID]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public DataTable UpdateFormInfo(string FormID, string styleTypeID, string campaign, string Form, string source)
        {
            String[] paramNames = { "Form_ID", "StyleType_ID", "Tracking_Campaign", "Tracking_Form", "Tracking_Source" };
            Object[] paramValues = { FormID, styleTypeID, campaign, Form, source };

            DataSet ds = SQL_SP_Exec("[spr_UpdateFormInfo]", con, paramNames, paramValues);

            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        #endregion
    }
}