using BabySpa.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using BabySpa.Models;
using System.Data;
using System.Data.SqlClient;

namespace BabySpa.Areas.Admin.Models
{
    public class AdminDBContext
    {
        static Result _result;
        //********************** Order **********************//
        #region Order
        public static Result OrderList(OrderSearch search )
        {
            StringBuilder cond = new StringBuilder();
            if (search.Status !=null && search.Status.Length >0)
            {
                cond.Append(" and o.status in (" + Helper.ArrayToStr(search.Status).Replace(";",",") + ")");
            }
            if (search.TourID != null && search.TourID != "")
            {
                cond.Append(" and o.tourid like '%" + search.TourID+"%'");
            }
            if (Func.ToInt(search.OrderID) > 0)
            {
                cond.Append(" and o.OrderID=" + search.OrderID.ToString());
            }
            if (Func.ToInt(search.CustID) >0)
            {
                cond.Append(" and c.CustID=" + search.CustID.ToString());
            }
            if (search.CustName != null && search.CustName != "")
            {
                cond.Append(" and (c.FName like '%" + search.CustName+ "%' or c.LName like '%" + search.CustName + "%')") ;
            }
            if (search.Email != null && search.Email != "")
            {
                cond.Append(" and c.email like '%" + search.Email + "%'");
            }
            if (Func.ToInt(search.StaffID) > 0)
            {
                cond.Append(" and o.StaffID=" + search.StaffID.ToString());
            }
            if (Func.ToInt(search.AgentID) > 0)
            {
                cond.Append(" and o.AgentID=" + search.AgentID.ToString());
            }
            if (Func.ToInt(search.TourSeason) > 0)
            {
                cond.Append(" and t.TourSeason=" + search.TourSeason.ToString());
            }
            if (search.TourType != null && search.TourType.Length > 0)
            {
                cond.Append(" and t.TourType in (" + Helper.ArrayToStr(search.TourType).Replace(";",",") + ")");
            }
            if (search.SOrderDate !=null && search.SOrderDate > default(DateTime))
            {
                cond.Append(" and OrderDate>='" + Func.ToDateStr(search.SOrderDate)+ "'");
            }
            if (search.EOrderDate != null && search.EOrderDate > default(DateTime))
            {
                cond.Append(" and OrderDate<='" + Func.ToDateStr(search.EOrderDate) + "'");
            }
            if (search.SCompletedDate != null && search.SCompletedDate > default(DateTime))
            {
                cond.Append(" and CompletedDate>='" + Func.ToDateStr(search.SCompletedDate) + "'");
            }
            if (search.ECompletedDate != null && search.ECompletedDate > default(DateTime))
            {
                cond.Append(" and CompletedDate<='" + Func.ToDateStr(search.ECompletedDate) + "'");
            }

            if (search.STourDate != null && search.STourDate > default(DateTime))
            {
                cond.Append(" and T.StartDate>='" + Func.ToDateStr(search.STourDate) + "'");
            }
            if (search.ETourDate != null && search.ETourDate > default(DateTime))
            {
                cond.Append(" and T.StartDate<='" + Func.ToDateStr(search.ETourDate) + "'");
            }
            string sql = cond.ToString();
            if (sql.Length>0)
            {
                sql = " where "+ sql.TrimStart(" and".ToCharArray());
            }
            return OrderList(sql);
        }
        
        public static Result OrderList(string condition="")
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"Select distinct o.*, t.tourtype, TourName 
                        from cust_Order o 
                        left join 
	                        (select tourid, TourName, startdate, enddate, tourseason, tourtype from TOUR
	                        union
	                        select tourid, TourName, startdate, enddate, tourseason, 3 from TOUR_CUSTOM) t 
                        on o.tourid = t.tourid
                        left join CUST c on o.OrderID = c.OrderID");
            if (condition!="")
            {
                sql.Append(condition);
            }
            sql.Append(" order by o.orderdate desc");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetOrder(int id)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"Select o.*, t.tourtype, TourName 
                        from cust_Order o 
                        left join 
	                        (select tourid, TourName, startdate, enddate, tourseason, tourtype from TOUR
	                        union
	                        select tourid, TourName, startdate, enddate, tourseason, 3 from TOUR_CUSTOM) t 
                        on o.tourid = t.tourid
                         where o.orderid = @OrderID;
                        select * from cust_payment p where p.orderid = @OrderID;
                        select * From send_emails where orderid = @OrderID;");
            SqlParameter pOrderID = new SqlParameter("@OrderID", SqlDbType.Int);
            pOrderID.Value = id;


            _result = Main.DataSetExecute(sql.ToString(),new SqlParameter[] {
                                                                        pOrderID});
            return _result;
        }
        public static Result Save(Order order)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE [CUST_ORDER] SET
                                [TotalAmount] =  @TotalAmount
                                ,[Cur] = @Cur
                                ,[TotalAmountMNT] =  @TotalAmountMNT
                                ,[Status] = @Status
                                ,[CompletedDate] = @CompletedDate
                                ,[StaffID] = @StaffID
                                ,[ParticipantCount] =  @ParticipantCount
                                ,[AdminNote] =  @AdminNote
                    WHERE ORDERID = @OrderID";
            
            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pOrderID = new SqlParameter("@OrderID", SqlDbType.Int);
            SqlParameter pTotalAmount = new SqlParameter("@TotalAmount", SqlDbType.Decimal);
            SqlParameter pCur = new SqlParameter("@Cur", SqlDbType.Char, 3);
            SqlParameter pTotalAmountMNT = new SqlParameter("@TotalAmountMNT", SqlDbType.Decimal);
            SqlParameter pStatus = new SqlParameter("@Status", SqlDbType.Int);
            SqlParameter pCompletedDate = new SqlParameter("@CompletedDate", SqlDbType.DateTime);
            SqlParameter pStaffID = new SqlParameter("@StaffID", SqlDbType.Int);
            SqlParameter pParticipantCount = new SqlParameter("@ParticipantCount", SqlDbType.Int);
            SqlParameter pAdminNote = new SqlParameter("@AdminNote", SqlDbType.NVarChar,1000);

            //pOrderID.Value = order.OrderID;
            //pTotalAmount.Value = order.TotalAmount;
            //pCur.Value = order.Cur;
            //pTotalAmountMNT.Value = order.TotalAmountMNT;
            //pStatus.Value = order.Status;
            //pCompletedDate.Value = order.CompletedDate;
            //pStaffID.Value = order.StaffID; 
            //pParticipantCount.Value = order.ParticipantCount;
            //pAdminNote.Value = order.AdminNote;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                        pOrderID,
                                                                        pTotalAmount,
                                                                        pCur,
                                                                        pTotalAmountMNT,
                                                                        pStatus,
                                                                        pCompletedDate,
                                                                        pStaffID,
                                                                        pParticipantCount,
                                                                        pAdminNote});

            return _result;
        }

        public static Result Delete(int id)
        {
            #region SQL
            string sqlCust = @"
                    
                    DELETE FROM [CUST_ORDER]
                    WHERE ORDERID = @OrderID";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pOrderID = new SqlParameter("@OrderID", SqlDbType.Int);
           
            pOrderID.Value = id;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                        pOrderID
                                                                       });

            return _result;
        }

        public static Result ChangeStatus(int id, int status)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE [CUST_ORDER] SET
                   [Status] = @Status WHERE ORDERID = @OrderID";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pOrderID = new SqlParameter("@OrderID", SqlDbType.Int);
            SqlParameter pStatus = new SqlParameter("@Status", SqlDbType.Int);

            pOrderID.Value = id;
            pStatus.Value = status;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                        pStatus,
                                                                        pOrderID
                                                                       });

            return _result;
        }
        #endregion

        //********************** Cust **********************//
        #region Customer
        public static Result CustList(CustSearch search)
        {
            StringBuilder cond = new StringBuilder();
           
            if (search.TourID != null && search.TourID != "")
            {
                cond.Append(" and c.tourid like '%" + search.TourID + "%'");
            }
            if (Func.ToInt(search.OrderID) > 0)
            {
                cond.Append(" and C.OrderID=" + search.OrderID.ToString());
            }
            if (Func.ToInt(search.CustID) > 0)
            {
                cond.Append(" and c.CustID=" + search.CustID.ToString());
            }
            if (search.CustName != null && search.CustName != "")
            {
                cond.Append(" and (c.FName like '%" + search.CustName + "%' or c.LName like '%" + search.CustName + "%')");
            }
            if (search.Email != null && search.Email != "")
            {
                cond.Append(" and c.email like '%" + search.Email + "%'");
            }
            if (Func.ToInt(search.AgentID) > 0)
            {
                cond.Append(" and o.AgentID=" + search.AgentID.ToString());
            }
            if (Func.ToInt(search.Country) > 0)
            {
                cond.Append(" and c.Country=" + search.Country.ToString());
            }
            if (Func.ToInt(search.TourSeason) > 0)
            {
                cond.Append(" and t.TourSeason=" + search.TourSeason.ToString());
            }
            if (search.SRegDate != null && search.SRegDate > default(DateTime))
            {
                cond.Append(" and RegDate>='" + Func.ToDateStr(search.SRegDate) + "'");
            }
            if (search.ERegDate != null && search.ERegDate > default(DateTime))
            {
                cond.Append(" and RegDate<='" + Func.ToDateStr(search.ERegDate) + "'");
            }
            if (search.SArriveDate != null && search.SArriveDate > default(DateTime))
            {
                cond.Append(" and ArriveDate>='" + Func.ToDateStr(search.SArriveDate) + "'");
            }
            if (search.EArriveDate != null && search.EArriveDate > default(DateTime))
            {
                cond.Append(" and ArriveDate<='" + Func.ToDateStr(search.EArriveDate) + "'");
            }

            if (search.STourDate != null && search.STourDate > default(DateTime))
            {
                cond.Append(" and T.StartDate>='" + Func.ToDateStr(search.STourDate) + "'");
            }
            if (search.ETourDate != null && search.ETourDate > default(DateTime))
            {
                cond.Append(" and T.StartDate<='" + Func.ToDateStr(search.ETourDate) + "'");
            }
            string sql = cond.ToString();
            if (sql.Length > 0)
            {
                sql = " where " + sql.TrimStart(" and".ToCharArray());
            }
            return CustList(sql);
        }

        public static Result CustList(string condition = "")
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"Select c.*, o.orderid, o.agentid, t.tourid, TourName 
                        from CUST c 
                        left join cust_Order o on o.orderid = c.orderid
                         left join 
	                        (select tourid, TourName, startdate, enddate, tourseason, tourtype from TOUR
	                        union
	                        select tourid, TourName, startdate, enddate, tourseason, 3 from TOUR_CUSTOM) t 
                        on o.tourid = t.tourid");
            if (condition != "")
            {
                sql.Append(condition);
            }
            sql.Append(" order by c.regDate desc");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }
        public static Result GetCustomer(int id)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"Select c.*, o.agentid, t.tourid, TourName,coverdatetext 
                        from CUST c 
                        left join cust_Order o on o.OrderID = c.OrderID
                         left join 
	                        (select tourid, TourName, startdate, enddate, tourseason, tourtype,coverdatetext from TOUR
	                        union
	                        select c.tourid, COALESCE(c.TourName,t.TourName), c.startdate, c.enddate, c.tourseason, 3, t.CoverDateText from TOUR_CUSTOM c
	                        left join TOUR t on c.ParentTourID = t.TOURID) t 
                        on o.tourid = t.tourid where c.custid = @pCustID;
                        select * from cust_requirement r where r.custid = @pCustID;
                        select * from cust_payment p where p.custid = @pCustID;
                        select * From send_emails where custid = @pCustID;
                        select g.* from CUST_GROUP g
                            left join CUST_ORDER o on g.TourID = o.TourID
	                        left join Cust c on o.orderid= c.orderid
	                    where c.custid =@pCustID;");
            SqlParameter pCustID = new SqlParameter("@pCustID", SqlDbType.Int);
            pCustID.Value = id;


            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pCustID});
            return _result;
        }
        public static Result CustSave(Customer cust)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE [CUST] SET
                              [GroupID] = @GroupID
                              ,[Nationality] = @Nationality
                              ,[Country] = @Country
                              ,[Address] = @Address
                              ,[Phone] = @Phone
                              ,[Email] = @Email
                              ,[SocialID] = @SocialID
                              ,[Title] = @Title
                              ,[FName] = @FName
                              ,[LName] = @LName
                              ,[Gender] = @Gender
                              ,[BirthDate] = @BirthDate
                              ,[Languages] = @Languages
                              ,[PassportNo] = @PassportNo
                              ,[PassportValidDate] = @PassportValidDate
                              ,[IsGroupLeader] = @IsGroupLeader
                              ,[UBFlightRequired] = @UBFlightRequired
                              ,[ExtraRoomRequired] = @ExtraRoomRequired
                              ,[JoinGroup] = @JoinGroup
                              ,[ArriveDate] = @ArriveDate
                              ,[DepartureDate] = @DepartureDate
                              ,[MealRequirement] = @MealRequirement
                              ,[MedicalCondition] = @MedicalCondition
                              ,[Comments] = @Comments
                              ,[AdminNote] = @AdminNote
                    WHERE CUSTID = @CustID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pCustID = new SqlParameter("@CustID", SqlDbType.Int);
            SqlParameter pGroupID = new SqlParameter("@GroupID", SqlDbType.Int);
            SqlParameter pNationality = new SqlParameter("@Nationality", SqlDbType.NVarChar, 50);
            SqlParameter pCountry = new SqlParameter("@Country", SqlDbType.Int);
            SqlParameter pAddress = new SqlParameter("@Address", SqlDbType.NVarChar, 150);
            SqlParameter pPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
            SqlParameter pEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 150);
            SqlParameter pSocialID = new SqlParameter("@SocialID", SqlDbType.NVarChar, 250);
            SqlParameter pTitle = new SqlParameter("@Title", SqlDbType.Int);
            SqlParameter pFName = new SqlParameter("@FName", SqlDbType.NVarChar, 100);
            SqlParameter pLName = new SqlParameter("@LName", SqlDbType.NVarChar, 100);
            SqlParameter pGender = new SqlParameter("@Gender", SqlDbType.Bit);
            SqlParameter pBirthDate = new SqlParameter("@BirthDate", SqlDbType.Date);
            SqlParameter pLanguages = new SqlParameter("@Languages", SqlDbType.NVarChar, 50);
            SqlParameter pPassportNo = new SqlParameter("@PassportNo", SqlDbType.NVarChar, 20);
            SqlParameter pPassportValidDate = new SqlParameter("@PassportValidDate", SqlDbType.NVarChar, 20);
            SqlParameter pIsGroupLeader = new SqlParameter("@IsGroupLeader", SqlDbType.Bit);
            SqlParameter pUBFlightRequired = new SqlParameter("@UBFlightRequired", SqlDbType.Bit);
            SqlParameter pExtraRoomRequired = new SqlParameter("@ExtraRoomRequired", SqlDbType.Bit);
            SqlParameter pJoinGroup = new SqlParameter("@JoinGroup", SqlDbType.Bit);
            SqlParameter pArriveDate = new SqlParameter("@ArriveDate", SqlDbType.DateTime);
            SqlParameter pDepartureDate = new SqlParameter("@DepartureDate", SqlDbType.DateTime);
            SqlParameter pMealRequirement = new SqlParameter("@MealRequirement", SqlDbType.NVarChar, 1000);
            SqlParameter pMedicalCondition = new SqlParameter("@MedicalCondition", SqlDbType.NVarChar, 1000);
            SqlParameter pComments = new SqlParameter("@Comments", SqlDbType.NVarChar, 4000);
            SqlParameter pAdminNote = new SqlParameter("@AdminNote", SqlDbType.NVarChar, 1000);

            #endregion
            #region Fill Params
            pCustID.Value = cust.CustID;
            pGroupID.Value = cust.GroupID;
            pNationality.Value = cust.Nationality;
            pCountry.Value = cust.Country;
            pAddress.Value = cust.Address;
            pPhone.Value = cust.Phone;
            pEmail.Value = cust.Email;
            pSocialID.Value = cust.SocialID;
            pTitle.Value = cust.Title;
            pFName.Value = cust.FName;
            pLName.Value = cust.LName;
            pGender.Value = cust.Gender;
            pBirthDate.Value = cust.BirthDate;
            pLanguages.Value = cust.Languages;
            pPassportNo.Value = cust.PassportNo;
            pPassportValidDate.Value = cust.PassportValidDate;
            pIsGroupLeader.Value = cust.IsGroupLeader;
            pUBFlightRequired.Value = Func.ToInt(cust.UBFlightRequired);
            pExtraRoomRequired.Value = Func.ToInt(cust.ExtraRoomRequired);
            pJoinGroup.Value = cust.JoinGroup;
            pArriveDate.Value = cust.ArriveDate;
            pDepartureDate.Value = cust.DepartureDate;
            pMealRequirement.Value = cust.MealRequirement;
            pMedicalCondition.Value = cust.MedicalCondition;
            pComments.Value = cust.Comments;
            pAdminNote.Value = cust.AdminNote;
            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                       pCustID,
                                                                        pGroupID,
                                                                        pNationality,
                                                                        pCountry,
                                                                        pAddress,
                                                                        pPhone,
                                                                        pEmail,
                                                                        pSocialID,
                                                                        pTitle,
                                                                        pFName,
                                                                        pLName,
                                                                        pGender,
                                                                        pBirthDate,
                                                                        pLanguages,
                                                                        pPassportNo,
                                                                        pPassportValidDate,
                                                                        pIsGroupLeader,
                                                                        pUBFlightRequired,
                                                                        pExtraRoomRequired,
                                                                        pJoinGroup,
                                                                        pArriveDate,
                                                                        pDepartureDate,
                                                                        pMealRequirement,
                                                                        pMedicalCondition,
                                                                        pComments,pAdminNote});

            return _result;
        }
        #endregion

        // ******************** Destination ****************//
        #region Destination
        public static Result GetDestinationList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_destination
                            order by DestinationName;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetDestinationDetail(int DestinationID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_destination
                            where Destinationid= @DestinationID");
            SqlParameter pDestinationID = new SqlParameter("@DestinationID", SqlDbType.Int);
            pDestinationID.Value = DestinationID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pDestinationID});
            return _result;
        }
        public static Result DestinationInsert(Destination item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @DestinationID= COALESCE(MAX(DestinationID),0)+1 from data_destination;
                        INSERT INTO [data_destination]
                                   ([DestinationID],
                                    [DestinationName],
                                    [DestinationDesc],
                                    [GeoLocation],
                                    [Province],
                                    [Soum],
                                    [PhotoUrl],
                                    [PhotoUrl2],
                                    [PhotoUrl3])
                                 VALUES
                                       (@DestinationID,
                                        @DestinationName,
                                        @DestinationDesc,
                                        @GeoLocation,
                                        @Province,
                                        @Soum,
                                        @PhotoUrl,
                                        @PhotoUrl2,
                                        @PhotoUrl3); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Destination Parameters
            SqlParameter pDestinationID = new SqlParameter("@DestinationID", SqlDbType.Int);
            SqlParameter pDestinationName = new SqlParameter("@DestinationName", SqlDbType.NVarChar, 150);
            SqlParameter pDestinationDesc = new SqlParameter("@DestinationDesc", SqlDbType.NVarChar,4000);
            SqlParameter pGeoLocation = new SqlParameter("@GeoLocation", SqlDbType.NVarChar, 100);
            SqlParameter pProvince = new SqlParameter("@Province", SqlDbType.NVarChar, 50);
            SqlParameter pSoum = new SqlParameter("@Soum", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);



            pDestinationID.Direction = ParameterDirection.Output;

            pDestinationName.Value = item.DestinationName;
            pDestinationDesc.Value = item.DestinationDesc;
            pGeoLocation.Value = item.GeoLocation;
            pProvince.Value = item.Province;
            pSoum.Value = item.Soum;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                   pDestinationID,
                                                                    pDestinationName,
                                                                    pDestinationDesc,
                                                                    pGeoLocation,
                                                                    pProvince,
                                                                    pSoum,
                                                                    pPhotoUrl,
                                                                    pPhotoUrl2,
                                                                    pPhotoUrl3,
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pDestinationID.Value;
            }
            return _result;
        }

        public static Result DestinationUpdate(Destination item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE data_destination SET
                            [DestinationName] = @DestinationName,
                            [DestinationDesc] = @DestinationDesc,
                            [GeoLocation] = @GeoLocation,
                            [Province] = @Province,
                            [Soum] = @Soum,
                            [PhotoUrl] = @PhotoUrl,
                            [PhotoUrl2] = @PhotoUrl2,
                            [PhotoUrl3] = @PhotoUrl3
                    WHERE DestinationID = @DestinationID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pDestinationID = new SqlParameter("@DestinationID", SqlDbType.Int);
            SqlParameter pDestinationName = new SqlParameter("@DestinationName", SqlDbType.NVarChar, 150);
            SqlParameter pDestinationDesc = new SqlParameter("@DestinationDesc", SqlDbType.NVarChar, 4000);
            SqlParameter pGeoLocation = new SqlParameter("@GeoLocation", SqlDbType.NVarChar, 100);
            SqlParameter pProvince = new SqlParameter("@Province", SqlDbType.NVarChar, 50);
            SqlParameter pSoum = new SqlParameter("@Soum", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);


            pDestinationID.Value = item.DestinationID;
            pDestinationName.Value = item.DestinationName;
            pDestinationDesc.Value = item.DestinationDesc;
            pGeoLocation.Value = item.GeoLocation;
            pProvince.Value = item.Province;
            pSoum.Value = item.Soum;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                pDestinationID,
                                                                pDestinationName,
                                                                pDestinationDesc,
                                                                pGeoLocation,
                                                                pProvince,
                                                                pSoum,
                                                                pPhotoUrl,
                                                                pPhotoUrl2,
                                                                pPhotoUrl3,
                                                                });

            return _result;
        }

        public static Result DestinationDelete(int id)
        {
            SqlDataReader dr = null;
            string strSQL = null;
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pDestinationID = new SqlParameter("@DestinationID", SqlDbType.Int);

                pDestinationID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from data_destination
                        WHERE DestinationID = @DestinationID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'data_destination'  and (ColumnName in ('destinationname','destinationdesc' )) and KeyField = @DestinationID;";

                strSQL = @" select * from (
                                select 'TourID:' +TourID +' DayNo:' + DayNo Usage, 'TourItinerary' UsedWhere from TOUR_Itinerary where destination = @DestinationID or Destination2 = @DestinationID or Destination3 = @DestinationID 
                                 union 
                                select AttractionName Usage, 'Attraction' UsedWhere from DATA_ATTRACTION where destinationid =@destinationid
                                union 
                                select AccommodationName Usage, 'Accommodation' UsedWhere from DATA_ACCOMMODATION where destinationid =@destinationid) D";

                #endregion
                dr = Main.ExecuteReader(strSQL, new[] { pDestinationID });

                if (dr.Read())
                {
                    _result.Succeed = false;
                    _result.Desc = Helper.MSG_DELETE_AVOID + "[Used in "+ Func.ToStr(dr["UsedWhere"])+":"+ Func.ToStr(dr["Usage"])+"]";
                    return _result;
                }

                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pDestinationID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Destination Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            finally
            {
                if (dr!=null && !dr.IsClosed)
                {
                    dr.Close();
                }
                dr = null;
            }
            return _result;
        }
        #endregion

        // ******************** Accommodation ****************//
        #region Accommodation
        public static Result GetAccommodationList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_Accommodation
                            order by AccommodationName;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetAccommodationDetail(int AccommodationID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_Accommodation
                            where Accommodationid= @AccommodationID");
            SqlParameter pAccommodationID = new SqlParameter("@AccommodationID", SqlDbType.Int);
            pAccommodationID.Value = AccommodationID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pAccommodationID});
            return _result;
        }
        public static Result AccommodationInsert(Accommodation item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @AccommodationID= COALESCE(MAX(AccommodationID),0)+1 from data_Accommodation;
                        INSERT INTO [data_Accommodation]
                                   ([AccommodationID],
                                    [AccommodationName],
                                    [AccommodationDesc],
                                    [AccommodationType],
                                    [AccommodationClass],
                                    [DestinationID],
                                    [PhotoUrl],
                                    [PhotoUrl2],
                                    [PhotoUrl3])
                                 VALUES
                                       (@AccommodationID,
                                        @AccommodationName,
                                        @AccommodationDesc,
                                        0,
                                        0,
                                        0,
                                        @PhotoUrl,
                                        @PhotoUrl2,
                                        @PhotoUrl3); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Accommodation Parameters
            SqlParameter pAccommodationID = new SqlParameter("@AccommodationID", SqlDbType.Int);
            SqlParameter pAccommodationName = new SqlParameter("@AccommodationName", SqlDbType.NVarChar, 150);
            SqlParameter pAccommodationDesc = new SqlParameter("@AccommodationDesc", SqlDbType.NVarChar, 4000);
            //SqlParameter pAccommodationType = new SqlParameter("@AccommodationType", SqlDbType.Int);
            //SqlParameter pAccommodationClass = new SqlParameter("@AccommodationClass", SqlDbType.Int);
            //SqlParameter pDestinationID = new SqlParameter("@DestinationID", SqlDbType.Int);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);



            pAccommodationID.Direction = ParameterDirection.Output;

            pAccommodationName.Value = item.AccommodationName;
            pAccommodationDesc.Value = item.AccommodationDesc;

            //pAccommodationType.Value = item.DestinationID;
            //pAccommodationClass.Value = item.AccommodationClass;
            //pDestinationID.Value = item.DestinationID;

            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                   pAccommodationID,
                                                                    pAccommodationName,
                                                                    pAccommodationDesc,
                                                                    pPhotoUrl,
                                                                    pPhotoUrl2,
                                                                    pPhotoUrl3,
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pAccommodationID.Value;
            }
            return _result;
        }

        public static Result AccommodationUpdate(Accommodation item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE data_Accommodation SET
                            [AccommodationName] = @AccommodationName,
                            [AccommodationDesc] = @AccommodationDesc,
                            [PhotoUrl] = @PhotoUrl,
                            [PhotoUrl2] = @PhotoUrl2,
                            [PhotoUrl3] = @PhotoUrl3
                    WHERE AccommodationID = @AccommodationID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pAccommodationID = new SqlParameter("@AccommodationID", SqlDbType.Int);
            SqlParameter pAccommodationName = new SqlParameter("@AccommodationName", SqlDbType.NVarChar, 150);
            SqlParameter pAccommodationDesc = new SqlParameter("@AccommodationDesc", SqlDbType.NVarChar, 4000);
            //SqlParameter pAccommodationType = new SqlParameter("@AccommodationType", SqlDbType.Int);
            //SqlParameter pAccommodationClass = new SqlParameter("@AccommodationClass", SqlDbType.Int);
            //SqlParameter pDestinationID = new SqlParameter("@DestinationID", SqlDbType.Int);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);


            pAccommodationID.Value = item.AccommodationID;
            pAccommodationName.Value = item.AccommodationName;
            pAccommodationDesc.Value = item.AccommodationDesc;
            //pAccommodationType.Value = item.DestinationID;
            //pAccommodationClass.Value = item.AccommodationClass;
            //pDestinationID.Value = item.DestinationID;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                pAccommodationID,
                                                                pAccommodationName,
                                                                pAccommodationDesc,
                                                                pPhotoUrl,
                                                                pPhotoUrl2,
                                                                pPhotoUrl3,
                                                                });

            return _result;
        }

        public static Result AccommodationDelete(int id)
        {
            SqlDataReader dr = null;
            string strSQL = null;
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pAccommodationID = new SqlParameter("@AccommodationID", SqlDbType.Int);

                pAccommodationID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from data_Accommodation
                        WHERE AccommodationID = @AccommodationID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'data_Accommodation'  and (ColumnName in ('accommodationname','accommodationdesc' )) and KeyField = @AccommodationID;";

                strSQL = @" select * from (
                                select 'TourID:' +TourID +' DayNo:' + DayNo Usage, 'TourItinerary' UsedWhere from TOUR_Itinerary where Accommodation = @AccommodationID or Accommodation2 = @AccommodationID or Accommodation3 = @AccommodationID 
                               ) D";
                #endregion

                dr = Main.ExecuteReader(strSQL, new[] { pAccommodationID });

                if (dr.Read())
                {
                    _result.Succeed = false;
                    _result.Desc = Helper.MSG_DELETE_AVOID + "[Used in " + Func.ToStr(dr["UsedWhere"]) + ":" + Func.ToStr(dr["Usage"]) + "]";
                    return _result;
                }

                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pAccommodationID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Accommodation Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                dr = null;
            }
            return _result;
        }
        #endregion

        // ******************** Food ****************//
        #region Food
        public static Result GetFoodList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_Food
                            order by FoodName;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetFoodDetail(int FoodID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_Food
                            where Foodid= @FoodID");
            SqlParameter pFoodID = new SqlParameter("@FoodID", SqlDbType.Int);
            pFoodID.Value = FoodID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pFoodID});
            return _result;
        }
        public static Result FoodInsert(Food item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @FoodID= COALESCE(MAX(FoodID),0)+1 from data_Food;
                        INSERT INTO [data_Food]
                                   ([FoodID],
                                    [FoodName],
                                    [FoodDesc],
                                    [FoodType],
                                    [PhotoUrl],
                                    [PhotoUrl2],
                                    [PhotoUrl3])
                                 VALUES
                                       (@FoodID,
                                        @FoodName,
                                        @FoodDesc,
                                        0,
                                        @PhotoUrl,
                                        @PhotoUrl2,
                                        @PhotoUrl3); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Food Parameters
            SqlParameter pFoodID = new SqlParameter("@FoodID", SqlDbType.Int);
            SqlParameter pFoodName = new SqlParameter("@FoodName", SqlDbType.NVarChar, 150);
            SqlParameter pFoodDesc = new SqlParameter("@FoodDesc", SqlDbType.NVarChar, 4000);
            //SqlParameter pFoodType = new SqlParameter("@FoodType", SqlDbType.Int);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);



            pFoodID.Direction = ParameterDirection.Output;

            pFoodName.Value = item.FoodName;
            pFoodDesc.Value = item.FoodDesc;
            //pFoodType.Value = item.Food;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                   pFoodID,
                                                                    pFoodName,
                                                                    pFoodDesc,
                                                                    pPhotoUrl,
                                                                    pPhotoUrl2,
                                                                    pPhotoUrl3,
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pFoodID.Value;
            }
            return _result;
        }

        public static Result FoodUpdate(Food item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE data_Food SET
                            [FoodName] = @FoodName,
                            [FoodDesc] = @FoodDesc,
                            [PhotoUrl] = @PhotoUrl,
                            [PhotoUrl2] = @PhotoUrl2,
                            [PhotoUrl3] = @PhotoUrl3
                    WHERE FoodID = @FoodID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pFoodID = new SqlParameter("@FoodID", SqlDbType.Int);
            SqlParameter pFoodName = new SqlParameter("@FoodName", SqlDbType.NVarChar, 150);
            SqlParameter pFoodDesc = new SqlParameter("@FoodDesc", SqlDbType.NVarChar, 4000);
            //SqlParameter pFoodType = new SqlParameter("@FoodType", SqlDbType.Int);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);


            pFoodID.Value = item.FoodID;
            pFoodName.Value = item.FoodName;
            pFoodDesc.Value = item.FoodDesc;
            //pFoodType.Value = item.FoodType;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                pFoodID,
                                                                pFoodName,
                                                                pFoodDesc,
                                                                pPhotoUrl,
                                                                pPhotoUrl2,
                                                                pPhotoUrl3,
                                                                });

            return _result;
        }

        public static Result FoodDelete(int id)
        {
            SqlDataReader dr = null;
            string strSQL = null;
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pFoodID = new SqlParameter("@FoodID", SqlDbType.Int);

                pFoodID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from data_Food
                        WHERE FoodID = @FoodID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'data_Food'  and (ColumnName in ('foodname','fooddesc' )) and KeyField = @FoodID;";

                strSQL = @" select * from (
                                select 'TourID:' +TourID +' DayNo:' + DayNo Usage, 'TourItinerary' UsedWhere 
                                        from TOUR_Itinerary 
                                    where Breakfast = @FoodID or Breakfast2 = @FoodID or Breakfast3 = @FoodID 
                                        or Lunch = @FoodID or Lunch2 = @FoodID or Lunch3 = @FoodID
                                        or Dinner = @FoodID or Dinner2 = @FoodID or Dinner3 = @FoodID) D";
                #endregion

                dr = Main.ExecuteReader(strSQL, new[] { pFoodID });

                if (dr.Read())
                {
                    _result.Succeed = false;
                    _result.Desc = Helper.MSG_DELETE_AVOID + "[Used in " + Func.ToStr(dr["UsedWhere"]) + ":" + Func.ToStr(dr["Usage"]) + "]";
                    return _result;
                }

                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pFoodID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Food Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                dr = null;
            }
            return _result;
        }
        #endregion

        // ******************** Transport ****************//
        #region Transport
        public static Result GetTransportList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_Transport
                            order by TransportName;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetTransportDetail(int TransportID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from data_Transport
                            where Transportid= @TransportID");
            SqlParameter pTransportID = new SqlParameter("@TransportID", SqlDbType.Int);
            pTransportID.Value = TransportID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTransportID});
            return _result;
        }
        public static Result TransportInsert(Transport item)
        {
            if (Func.ToStr(item.TransportDesc)=="")
            {
                item.TransportDesc = "-";
            }
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @TransportID= COALESCE(MAX(TransportID),0)+1 from data_Transport;
                        INSERT INTO [data_Transport]
                                   ([TransportID],
                                    [TransportName],
                                    [TransportDesc],
                                    [Brand],
                                    [mark],
                                    [Type],
                                    [PhotoUrl],
                                    [PhotoUrl2],
                                    [PhotoUrl3])
                                 VALUES
                                       (@TransportID,
                                        @TransportName,
                                        @TransportDesc,
                                        @Brand,
                                        @Mark,
                                        @Type,
                                        @PhotoUrl,
                                        @PhotoUrl2,
                                        @PhotoUrl3); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Transport Parameters
            SqlParameter pTransportID = new SqlParameter("@TransportID", SqlDbType.Int);
            SqlParameter pTransportName = new SqlParameter("@TransportName", SqlDbType.NVarChar, 150);
            SqlParameter pTransportDesc = new SqlParameter("@TransportDesc", SqlDbType.NVarChar, 4000);
            SqlParameter pBrand= new SqlParameter("@Brand", SqlDbType.NVarChar, 50);
            SqlParameter pMark = new SqlParameter("@Mark", SqlDbType.NVarChar, 50);
            SqlParameter pType = new SqlParameter("@Type", SqlDbType.Int);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);



            pTransportID.Direction = ParameterDirection.Output;

            pTransportName.Value = item.TransportName;
            pTransportDesc.Value = item.TransportDesc;
            pBrand.Value = item.Brand;
            pMark.Value = item.Mark;
            pType.Value = item.Type;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                   pTransportID,
                                                                    pTransportName,
                                                                    pTransportDesc,
                                                                    pBrand,
                                                                    pMark,
                                                                    pType,
                                                                    pPhotoUrl,
                                                                    pPhotoUrl2,
                                                                    pPhotoUrl3,
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pTransportID.Value;
            }
            return _result;
        }

        public static Result TransportUpdate(Transport item)
        {
            if (Func.ToStr(item.TransportDesc) == "")
            {
                item.TransportDesc = "-";
            }
            #region SQL
            string sqlCust = @"
                    
                    UPDATE data_Transport SET
                            [TransportName] = @TransportName,
                            [TransportDesc] = @TransportDesc,
                            [Brand] = @Brand,
                            [Mark] = @Mark,
                            [Type] = @Type,
                            [PhotoUrl] = @PhotoUrl,
                            [PhotoUrl2] = @PhotoUrl2,
                            [PhotoUrl3] = @PhotoUrl3
                    WHERE TransportID = @TransportID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pTransportID = new SqlParameter("@TransportID", SqlDbType.Int);
            SqlParameter pTransportName = new SqlParameter("@TransportName", SqlDbType.NVarChar, 150);
            SqlParameter pTransportDesc = new SqlParameter("@TransportDesc", SqlDbType.NVarChar, 4000);
            SqlParameter pBrand = new SqlParameter("@Brand", SqlDbType.NVarChar, 50);
            SqlParameter pMark = new SqlParameter("@Mark", SqlDbType.NVarChar, 50);
            SqlParameter pType = new SqlParameter("@Type", SqlDbType.Int);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl2 = new SqlParameter("@PhotoUrl2", SqlDbType.NVarChar, 250);
            SqlParameter pPhotoUrl3 = new SqlParameter("@PhotoUrl3", SqlDbType.NVarChar, 250);


            pTransportID.Value = item.TransportID;
            pTransportName.Value = item.TransportName;
            pTransportDesc.Value = item.TransportDesc;
            pBrand.Value = item.Brand;
            pMark.Value = item.Mark;
            pType.Value = item.Type;
            pPhotoUrl.Value = item.PhotoUrl[0];
            pPhotoUrl2.Value = item.PhotoUrl[1];
            pPhotoUrl3.Value = item.PhotoUrl[2];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                pTransportID,
                                                                pTransportName,
                                                                pTransportDesc,
                                                                pBrand,
                                                                pMark,
                                                                pType,
                                                                pPhotoUrl,
                                                                pPhotoUrl2,
                                                                pPhotoUrl3,
                                                                });

            return _result;
        }

        public static Result TransportDelete(int id)
        {
            SqlDataReader dr = null;
            string strSQL = null;
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pTransportID = new SqlParameter("@TransportID", SqlDbType.Int);

                pTransportID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from data_Transport
                        WHERE TransportID = @TransportID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'data_Transport'  and (ColumnName in ('transportname','transportdesc' )) and KeyField = @TransportID;";

                strSQL = @" select * from (
                                select 'TourID:' +TourID +' DayNo:' + DayNo Usage, 'TourItinerary' UsedWhere from TOUR_Itinerary 
                                    where Transport = @TransportID 
                                ) D";

                #endregion
                dr = Main.ExecuteReader(strSQL, new[] { pTransportID });

                if (dr.Read())
                {
                    _result.Succeed = false;
                    _result.Desc = Helper.MSG_DELETE_AVOID + "[Used in " + Func.ToStr(dr["UsedWhere"]) + ":" + Func.ToStr(dr["Usage"]) + "]";
                    return _result;
                }

                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pTransportID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Transport Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                dr = null;
            }
            return _result;
        }
        #endregion

        // ******************** Blog ****************//
        #region Blog
        public static Result GetBlogList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select [BlogID]
                              ,[BlogName]
                              ,[BlogDate]
                              ,[BlogType]
                              ,[Author]
                              ,[Tags]
                              ,[Category]
                              ,N'' BlogContent
                              ,[IsFeatured]
                              ,[TOURID]
                              ,[CoverUrl] from Blog b
                            order by BlogDate desc;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetBlogDetail(int BlogID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from Blog
                            where Blogid= @BlogID");
            SqlParameter pBlogID = new SqlParameter("@BlogID", SqlDbType.Int);
            pBlogID.Value = BlogID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pBlogID});
            return _result;
        }
        public static Result BlogInsert(Blog item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @BlogID= COALESCE(MAX(BlogID),0)+1 from Blog;
                        INSERT INTO [Blog]
                                   ([BlogID],
                                    [BlogName],
                                    [BlogDate],
                                    [BlogType],
                                    [Author],
                                    [Tags],
                                    [Category],
                                    [BlogContent],
                                    [IsFeatured],
                                    [TOURID],
                                    [CoverUrl])
                                 VALUES
                                       (@BlogID,
                                        @BlogName,
                                        @BlogDate,
                                        @BlogType,
                                        @Author,
                                        @Tags,
                                        @Category,
                                        @BlogContent,
                                        @IsFeatured,
                                        @TOURID,
                                        @CoverUrl
                                        ); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Blog Parameters
            SqlParameter pBlogID = new SqlParameter("@BlogID", SqlDbType.Int);
            SqlParameter pBlogName = new SqlParameter("@BlogName", SqlDbType.NVarChar, 150);
            SqlParameter pBlogDate = new SqlParameter("@BlogDate", SqlDbType.DateTime);
            SqlParameter pBlogType = new SqlParameter("@BlogType", SqlDbType.Int);
            SqlParameter pAutor = new SqlParameter("@Author", SqlDbType.NVarChar, 100);
            SqlParameter pTags = new SqlParameter("@Tags", SqlDbType.NVarChar, 200);
            SqlParameter pCategory = new SqlParameter("@Category", SqlDbType.Int);
            SqlParameter pBlogContent = new SqlParameter("@BlogContent", SqlDbType.NVarChar);
            SqlParameter pIsFeatured = new SqlParameter("@IsFeatured", SqlDbType.Int);
            SqlParameter pTOURID = new SqlParameter("@TOURID", SqlDbType.NVarChar, 50);
            SqlParameter pCoverUrl = new SqlParameter("@CoverUrl", SqlDbType.NVarChar, 250);


            pBlogID.Direction = ParameterDirection.Output;

            pBlogID.Value = item.BlogID;
            pBlogName.Value = item.BlogName;
            pBlogDate.Value = item.BlogDate;
            pBlogType.Value = item.BlogType;
            pAutor.Value = item.Author;
            pTags.Value = ";" + Helper.ArrayToStr(item.Tags,false);
            pCategory.Value = item.Category;
            pBlogContent.Value = item.BlogContent;
            pIsFeatured.Value = item.IsFeatured;
            pTOURID.Value = item.TOURID;
            pCoverUrl.Value = item.CoverUrl;



            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                  pBlogID,
                                                                    pBlogName,
                                                                    pBlogDate,
                                                                    pBlogType,
                                                                    pAutor,
                                                                    pTags,
                                                                    pCategory,
                                                                    pBlogContent,
                                                                    pIsFeatured,
                                                                    pTOURID,
                                                                    pCoverUrl
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pBlogID.Value;
            }
            return _result;
        }

        public static Result BlogUpdate(Blog item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE Blog SET
                           [BlogName] = @BlogName,	
                            [BlogDate] = @BlogDate,	
                            [BlogType] = @BlogType,	
                            [Author] = @Author,	
                            [Tags] = @Tags,	
                            [Category] = @Category,	
                            [BlogContent] = @BlogContent,	
                            [IsFeatured] = @IsFeatured,	
                            [TOURID] = @TOURID,	
                            [CoverUrl] = @CoverUrl
                    WHERE BlogID = @BlogID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pBlogID = new SqlParameter("@BlogID", SqlDbType.Int);
            SqlParameter pBlogName = new SqlParameter("@BlogName", SqlDbType.NVarChar, 150);
            SqlParameter pBlogDate = new SqlParameter("@BlogDate", SqlDbType.DateTime);
            SqlParameter pBlogType = new SqlParameter("@BlogType", SqlDbType.Int);
            SqlParameter pAutor = new SqlParameter("@Author", SqlDbType.NVarChar, 100);
            SqlParameter pTags = new SqlParameter("@Tags", SqlDbType.NVarChar, 200);
            SqlParameter pCategory = new SqlParameter("@Category", SqlDbType.Int);
            SqlParameter pBlogContent = new SqlParameter("@BlogContent", SqlDbType.NVarChar);
            SqlParameter pIsFeatured = new SqlParameter("@IsFeatured", SqlDbType.Int);
            SqlParameter pTOURID = new SqlParameter("@TOURID", SqlDbType.NVarChar, 50);
            SqlParameter pCoverUrl = new SqlParameter("@CoverUrl", SqlDbType.NVarChar, 250);


            pBlogID.Value = item.BlogID;
            pBlogName.Value = item.BlogName;
            pBlogDate.Value = item.BlogDate;
            pBlogType.Value = item.BlogType;
            pAutor.Value = item.Author;
            pTags.Value = ";"+ Helper.ArrayToStr(item.Tags, false);
            pCategory.Value = item.Category;
            pBlogContent.Value = item.BlogContent;
            pIsFeatured.Value = item.IsFeatured;
            pTOURID.Value = item.TOURID;
            pCoverUrl.Value = item.CoverUrl;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                               pBlogID,
                                                                    pBlogName,
                                                                    pBlogDate,
                                                                    pBlogType,
                                                                    pAutor,
                                                                    pTags,
                                                                    pCategory,
                                                                    pBlogContent,
                                                                    pIsFeatured,
                                                                    pTOURID,
                                                                    pCoverUrl
                                                                });

            return _result;
        }

        public static Result BlogDelete(int id)
        {
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pBlogID = new SqlParameter("@BlogID", SqlDbType.Int);

                pBlogID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from Blog
                        WHERE BlogID = @BlogID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'Blog'  and (ColumnName in ('blogname','blogcontent' )) and KeyField = @BlogID;";

                #endregion
                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pBlogID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Blog Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            return _result;
        }
        #endregion

        // ******************** Gallery ****************//
        #region Gallery
        public static Result GetGalleryList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select [GalleryID]
                              ,[GalleryName]
                              ,[GalleryDate]
                              ,[GalleryType]
                              ,[Author]
                              ,[Tags]
                              ,[Category]
                              ,N'' GalleryDesc
                              ,[IsFeatured]
                              ,[TOURID]
                              ,[CoverUrl] from Gallery b
                            order by GalleryDate desc;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetGalleryDetail(int GalleryID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"begin select * from Gallery
                            where Galleryid= @GalleryID;
                            select * from gallery_photo where galleryid = @GalleryID; end;");
            SqlParameter pGalleryID = new SqlParameter("@GalleryID", SqlDbType.Int);
            pGalleryID.Value = GalleryID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pGalleryID});
            return _result;
        }
        public static Result GalleryInsert(Gallery item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @GalleryID= COALESCE(MAX(GalleryID),0)+1 from Gallery;
                        INSERT INTO [Gallery]
                                   ([GalleryID],
                                    [GalleryName],
                                    [GalleryDate],
                                    [GalleryType],
                                    [Author],
                                    [Tags],
                                    [Category],
                                    [GalleryDesc],
                                    [IsFeatured],
                                    [TOURID],
                                    [CoverUrl])
                                 VALUES
                                       (@GalleryID,
                                        @GalleryName,
                                        @GalleryDate,
                                        @GalleryType,
                                        @Author,
                                        @Tags,
                                        @Category,
                                        @GalleryDesc,
                                        @IsFeatured,
                                        @TOURID,
                                        @CoverUrl
                                        ); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Gallery Parameters
            SqlParameter pGalleryID = new SqlParameter("@GalleryID", SqlDbType.Int);
            SqlParameter pGalleryName = new SqlParameter("@GalleryName", SqlDbType.NVarChar, 150);
            SqlParameter pGalleryDate = new SqlParameter("@GalleryDate", SqlDbType.DateTime);
            SqlParameter pGalleryType = new SqlParameter("@GalleryType", SqlDbType.Int);
            SqlParameter pAutor = new SqlParameter("@Author", SqlDbType.NVarChar, 100);
            SqlParameter pTags = new SqlParameter("@Tags", SqlDbType.NVarChar, 200);
            SqlParameter pCategory = new SqlParameter("@Category", SqlDbType.Int);
            SqlParameter pGalleryDesc = new SqlParameter("@GalleryDesc", SqlDbType.NVarChar);
            SqlParameter pIsFeatured = new SqlParameter("@IsFeatured", SqlDbType.Int);
            SqlParameter pTOURID = new SqlParameter("@TOURID", SqlDbType.NVarChar, 50);
            SqlParameter pCoverUrl = new SqlParameter("@CoverUrl", SqlDbType.NVarChar, 250);


            pGalleryID.Direction = ParameterDirection.Output;

            pGalleryID.Value = item.GalleryID;
            pGalleryName.Value = item.GalleryName;
            pGalleryDate.Value = item.GalleryDate;
            pGalleryType.Value = item.GalleryType;
            pAutor.Value = item.Author;
            pTags.Value = ";" + Helper.ArrayToStr(item.Tags, false);
            pCategory.Value = item.Category;
            pGalleryDesc.Value = item.GalleryDesc;
            pIsFeatured.Value = item.IsFeatured;
            pTOURID.Value = item.TOURID;
            pCoverUrl.Value = item.CoverUrl;



            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                  pGalleryID,
                                                                    pGalleryName,
                                                                    pGalleryDate,
                                                                    pGalleryType,
                                                                    pAutor,
                                                                    pTags,
                                                                    pCategory,
                                                                    pGalleryDesc,
                                                                    pIsFeatured,
                                                                    pTOURID,
                                                                    pCoverUrl
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pGalleryID.Value;
            }
            return _result;
        }

        public static Result GalleryUpdate(Gallery item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE Gallery SET
                           [GalleryName] = @GalleryName,	
                            [GalleryDate] = @GalleryDate,	
                            [GalleryType] = @GalleryType,	
                            [Author] = @Author,	
                            [Tags] = @Tags,	
                            [Category] = @Category,	
                            [GalleryDesc] = @GalleryDesc,	
                            [IsFeatured] = @IsFeatured,	
                            [TOURID] = @TOURID,	
                            [CoverUrl] = @CoverUrl
                    WHERE GalleryID = @GalleryID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pGalleryID = new SqlParameter("@GalleryID", SqlDbType.Int);
            SqlParameter pGalleryName = new SqlParameter("@GalleryName", SqlDbType.NVarChar, 150);
            SqlParameter pGalleryDate = new SqlParameter("@GalleryDate", SqlDbType.DateTime);
            SqlParameter pGalleryType = new SqlParameter("@GalleryType", SqlDbType.Int);
            SqlParameter pAutor = new SqlParameter("@Author", SqlDbType.NVarChar, 100);
            SqlParameter pTags = new SqlParameter("@Tags", SqlDbType.NVarChar, 200);
            SqlParameter pCategory = new SqlParameter("@Category", SqlDbType.Int);
            SqlParameter pGalleryDesc = new SqlParameter("@GalleryDesc", SqlDbType.NVarChar);
            SqlParameter pIsFeatured = new SqlParameter("@IsFeatured", SqlDbType.Int);
            SqlParameter pTOURID = new SqlParameter("@TOURID", SqlDbType.NVarChar, 50);
            SqlParameter pCoverUrl = new SqlParameter("@CoverUrl", SqlDbType.NVarChar, 250);


            pGalleryID.Value = item.GalleryID;
            pGalleryName.Value = item.GalleryName;
            pGalleryDate.Value = item.GalleryDate;
            pGalleryType.Value = item.GalleryType;
            pAutor.Value = item.Author;
            pTags.Value = ";" + Helper.ArrayToStr(item.Tags, false);
            pCategory.Value = item.Category;
            pGalleryDesc.Value = item.GalleryDesc;
            pIsFeatured.Value = item.IsFeatured;
            pTOURID.Value = item.TOURID;
            pCoverUrl.Value = item.CoverUrl;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                               pGalleryID,
                                                                    pGalleryName,
                                                                    pGalleryDate,
                                                                    pGalleryType,
                                                                    pAutor,
                                                                    pTags,
                                                                    pCategory,
                                                                    pGalleryDesc,
                                                                    pIsFeatured,
                                                                    pTOURID,
                                                                    pCoverUrl
                                                                });

            return _result;
        }

        public static Result GalleryDelete(int id)
        {
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pGalleryID = new SqlParameter("@GalleryID", SqlDbType.Int);

                pGalleryID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from Gallery
                        WHERE GalleryID = @GalleryID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'Gallery'  and (ColumnName in ('galleryname','gallerydesc' )) and KeyField = @GalleryID;";

                #endregion
                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pGalleryID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Gallery Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            return _result;
        }

        // ********************* Gallery Photos *******************//

        public static Result GetPhotoList(int Galleryid)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from Gallery_photo
                            where GalleryID = @GalleryID;");
            SqlParameter pGalleryID = new SqlParameter("@GalleryID", SqlDbType.Int);
            pGalleryID.Value = Galleryid;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pGalleryID,});
            return _result;
        }

        public static Result GetPhotoDetail(int photoID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from Gallery_photo
                            where photoid= @PhotoID");
            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);
            pPhotoID.Value = photoID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pPhotoID});
            return _result;
        }
        public static Result PhotoInsert(GalleryPhoto item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @PhotoID= COALESCE(MAX(PhotoID),0)+1 from Gallery_Photo;
                        INSERT INTO [Gallery_Photo]
                                   ([GalleryID]
                                   ,[PhotoName]
                                   ,[PhotoUrl]
                                   ,[IsVideo]
                                   ,[PhotoDesc]
                                   ,[PhotoID]
                                   ,[Tags])
                                 VALUES
                                       (@GalleryID,
                                        @PhotoName,
                                        @PhotoUrl,
                                        @IsVideo,
                                        @PhotoDesc,
                                        @PhotoID,
                                        @Tags); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Photo Parameters
            SqlParameter pGalleryID = new SqlParameter("@GalleryID", SqlDbType.Int);
            SqlParameter pPhotoName = new SqlParameter("@PhotoName", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pIsVideo = new SqlParameter("@IsVideo", SqlDbType.Int);
            SqlParameter pPhotoDesc = new SqlParameter("@PhotoDesc", SqlDbType.NVarChar, 150);
            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);
            SqlParameter pTags = new SqlParameter("@Tags", SqlDbType.NVarChar, 100);


            pPhotoID.Direction = ParameterDirection.Output;

            pGalleryID.Value = item.GalleryID;
            pPhotoName.Value = item.PhotoName;
            pPhotoUrl.Value = item.PhotoUrl;
            pIsVideo.Value = item.IsVideo;
            pPhotoDesc.Value = item.PhotoDesc;
            pTags.Value = ";" + Helper.ArrayToStr(item.Tags, false);

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                   pGalleryID,
                                                                    pPhotoName,
                                                                    pPhotoUrl,
                                                                    pIsVideo,
                                                                    pPhotoDesc,
                                                                    pPhotoID,
                                                                    pTags
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pPhotoID.Value;
            }
            return _result;
        }

        public static Result PhotoUpdate(GalleryPhoto item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE Gallery_Photo SET
                                [PhotoName] = @PhotoName,
                                [PhotoUrl] = @PhotoUrl,
                                [Author] = @Author,
                                [IsVideo] = @IsVideo,
                                [PhotoDesc] = @PhotoDesc,
                                [Tags] = @Tags
                    WHERE PhotoID = @PhotoID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pPhotoName = new SqlParameter("@PhotoName", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pAuthor = new SqlParameter("@Author", SqlDbType.NVarChar, 50);
            SqlParameter pIsVideo = new SqlParameter("@IsVideo", SqlDbType.Int);
            SqlParameter pPhotoDesc = new SqlParameter("@PhotoDesc", SqlDbType.NVarChar, 150);
            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);
            SqlParameter pTags = new SqlParameter("@Tags", SqlDbType.NVarChar, 100);


            pPhotoName.Value = item.PhotoName;
            pPhotoUrl.Value = item.PhotoUrl;
            pAuthor.Value = item.Author;
            pIsVideo.Value = item.IsVideo;
            pPhotoDesc.Value = item.PhotoDesc;
            pPhotoID.Value = item.PhotoID;
            pTags.Value = ";" + Helper.ArrayToStr(item.Tags, false);


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                pPhotoName,
                                                                pPhotoUrl,
                                                                pAuthor,
                                                                pIsVideo,
                                                                pPhotoDesc,
                                                                pPhotoID,
                                                                pTags,
                                                                    });

            return _result;
        }

        public static Result PhotoDelete(GalleryPhoto item)
        {
            #region SQL
            string sqlCust = @"
                    Delete from Gallery_Photo
                        WHERE PhotoID = @PhotoID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'gallery_photo'  and (ColumnName in ('photoname','photodesc' )) and KeyField = @PhotoID;";

            #endregion

            #region SQL Parameter Initiliaze

            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);

            pPhotoID.Value = item.PhotoID;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pPhotoID
                                                                    });

            return _result;
        }
        #endregion


        // ******************** Staff ****************//
        #region Staff
        public static Result GetStaffList()
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from SysUser
                            order by isActive desc, FirstName;");

            _result = Main.DataSetExecute(sql.ToString());
            return _result;
        }

        public static Result GetStaffDetail(int StafftID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from SysUser
                            where UserID= @StafftID");
            SqlParameter pStafftID = new SqlParameter("@StafftID", SqlDbType.Int);
            pStafftID.Value = StafftID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pStafftID});
            return _result;
        }
        public static Result StaffInsert(Staff item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @UserID= COALESCE(MAX(UserID),0)+1 from SysUser;
                        INSERT INTO [SysUser]
                                   ([UserID],
                                    [FirstName],
                                    [LastName],
                                    [Phone],
                                    [Email],
                                    [Position],
                                    [isactive],
                                    [Duty],
                                    [AvatarPhoto],
                                    [Department],
                                    [LoginName],
                                    [Password]
                                    )
                                 VALUES
                                       (@UserID,
                                    @FirstName,
                                    @LastName,
                                    @Phone,
                                    @Email,
                                    @Position,
                                    @isactive,
                                    @Duty,
                                    @AvatarPhoto,'Deparment',@FirstName,'1'); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Staff Parameters
            SqlParameter pUserID = new SqlParameter("@UserID", SqlDbType.Int);
            SqlParameter pFirstName = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            SqlParameter pLastName = new SqlParameter("@LastName", SqlDbType.NVarChar, 50);
            SqlParameter pPhone = new SqlParameter("@Phone", SqlDbType.NVarChar, 50);
            SqlParameter pEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
            SqlParameter pPosition = new SqlParameter("@Position", SqlDbType.NVarChar, 50);
            SqlParameter pisactive = new SqlParameter("@isactive", SqlDbType.Int);
            SqlParameter pDuty = new SqlParameter("@Duty", SqlDbType.NVarChar, 1000);
            SqlParameter pAvatarPhoto = new SqlParameter("@AvatarPhoto", SqlDbType.NVarChar, 250);




            pUserID.Direction = ParameterDirection.Output;

            pUserID.Value = item.StaffID;
            pFirstName.Value = item.StaffFName;
            pLastName.Value = item.StaffLName;
            pPhone.Value = item.Phone;
            pEmail.Value = item.EMail;
            pPosition.Value = item.Position;
            pisactive.Value = item.isActive;
            pDuty.Value = item.StaffDesc;
            pAvatarPhoto.Value = item.PhotoUrl[0];



            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                  pUserID,
                                                                pFirstName,
                                                                pLastName,
                                                                pPhone,
                                                                pEmail,
                                                                pPosition,
                                                                pisactive,
                                                                pDuty,
                                                                pAvatarPhoto,
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pUserID.Value;
            }
            return _result;
        }

        public static Result StaffUpdate(Staff item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE SysUser SET
                            [FirstName] = @FirstName,
                            [LastName] = @LastName,
                            [Phone] = @Phone,
                            [Email] = @Email,
                            [Position] = @Position,
                            [isactive] = @isactive,
                            [Duty] = @Duty,
                            [AvatarPhoto] = @AvatarPhoto
                    WHERE UserID = @UserID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pUserID = new SqlParameter("@UserID", SqlDbType.Int);
            SqlParameter pFirstName = new SqlParameter("@FirstName", SqlDbType.NVarChar, 50);
            SqlParameter pLastName = new SqlParameter("@LastName", SqlDbType.NVarChar,50);
            SqlParameter pPhone = new SqlParameter("@Phone", SqlDbType.NVarChar,50);
            SqlParameter pEmail = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
            SqlParameter pPosition = new SqlParameter("@Position", SqlDbType.NVarChar, 50);
            SqlParameter pisactive = new SqlParameter("@isactive", SqlDbType.Int);
            SqlParameter pDuty = new SqlParameter("@Duty", SqlDbType.NVarChar,1000);
            SqlParameter pAvatarPhoto = new SqlParameter("@AvatarPhoto", SqlDbType.NVarChar,250);


            pUserID.Value = item.StaffID;
            pFirstName.Value = item.StaffFName;
            pLastName.Value = item.StaffLName;
            pPhone.Value = item.Phone;
            pEmail.Value = item.EMail;
            pPosition.Value = item.Position;
            pisactive.Value = item.isActive;
            pDuty.Value = item.StaffDesc;
            pAvatarPhoto.Value = item.PhotoUrl[0];


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                               pUserID,
                                                                pFirstName,
                                                                pLastName,
                                                                pPhone,
                                                                pEmail,
                                                                pPosition,
                                                                pisactive,
                                                                pDuty,
                                                                pAvatarPhoto
                                                                });

            return _result;
        }

        public static Result StaffDelete(int id)
        {
            SqlDataReader dr = null;
            string strSQL = null;
            try
            {
                #region SQL Parameter Initiliaze

                SqlParameter pUserID = new SqlParameter("@UserID", SqlDbType.Int);

                pUserID.Value = id;

                #endregion
                #region SQL
                string sqlCust = @"
                    Delete from SysUser
                        WHERE UserID = @UserID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'SysUser'  and (ColumnName in ('FirstName','Duty' )) and KeyField = @UserID;";

                strSQL = @" select * from (
                                select 'TourID:' +TourID  Usage, 'Tour Info' UsedWhere from TOUR_Info
                                    where infotype='transport', infovalue2= @UserID 
                                ) D";

                #endregion
                dr = Main.ExecuteReader(strSQL, new[] { pUserID });

                if (dr.Read())
                {
                    _result.Succeed = false;
                    _result.Desc = Helper.MSG_DELETE_AVOID + "[Used in " + Func.ToStr(dr["UsedWhere"]) + ":" + Func.ToStr(dr["Usage"]) + "]";
                    return _result;
                }

                _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pUserID
                                                                    });
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Staff Delete", ex);
                _result.Desc = ex.Message;
                _result.Succeed = false;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                {
                    dr.Close();
                }
                dr = null;
            }
            return _result;
        }
        #endregion

        //******************* Translator /Globalization/ ***************//
        public static Result GetTranslatorList(string tablename, string columnname, string keyField)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from DATA_GLOBALIZATION
                            where tablename = @TableName and columnname = @ColumnName and keyField = @KeyField
                            order by Lang;");
            SqlParameter pTableName = new SqlParameter("@TableName", SqlDbType.NVarChar, 50);
            SqlParameter pColumnName = new SqlParameter("@ColumnName", SqlDbType.NVarChar, 50);
            SqlParameter pKeyField = new SqlParameter("@KeyField", SqlDbType.NVarChar, 50);
            

            pTableName.Value = tablename;
            pColumnName.Value = columnname;
            pKeyField.Value = keyField;



            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTableName,
                                                                        pColumnName,
                                                                        pKeyField });
            return _result;
        }

        public static Result GetTranslatorDetail(string tablename, string columnname, string keyField, string lang)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from DATA_GLOBALIZATION
                            where tablename = @TableName and columnname = @ColumnName and keyField = @KeyField and Lang=@lang");
            SqlParameter pTableName = new SqlParameter("@TableName", SqlDbType.NVarChar, 50);
            SqlParameter pColumnName = new SqlParameter("@ColumnName", SqlDbType.NVarChar, 50);
            SqlParameter pKeyField = new SqlParameter("@KeyField", SqlDbType.NVarChar, 50);
            SqlParameter pLang = new SqlParameter("@Lang", SqlDbType.NVarChar, 5);


            pTableName.Value = tablename;
            pColumnName.Value = columnname;
            pKeyField.Value = keyField;
            pLang.Value = lang;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTableName,
                                                                        pColumnName,
                                                                        pKeyField,
                                                                        pLang,
                                                                        });
            return _result;
        }
        public static Result TranslatorInsert(TranslatorValue item)
        {
           
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    INSERT INTO DATA_GLOBALIZATION
                           ([TableName]
                           ,[ColumnName]
                           ,[KeyField]
                           ,[Lang]
                           ,[TextValue])
                     VALUES
                           (@TableName,
                            @ColumnName,
                            @KeyField,
                            @Lang,
                            @TextValue); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pTableName = new SqlParameter("@TableName", SqlDbType.NVarChar, 50);
            SqlParameter pColumnName = new SqlParameter("@ColumnName", SqlDbType.NVarChar, 50);
            SqlParameter pKeyField = new SqlParameter("@KeyField", SqlDbType.NVarChar, 50);
            SqlParameter pLang = new SqlParameter("@Lang", SqlDbType.NVarChar, 5);
            SqlParameter pTextValue = new SqlParameter("@TextValue", SqlDbType.NVarChar);

            pTableName.Value = item.TableName;
            pColumnName.Value = item.ColumnName;
            pKeyField.Value = item.KeyField;
            pLang.Value = item.Lang;
            pTextValue.Value = item.TextValue;


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                  pTableName,
                                                                pColumnName,
                                                                pKeyField,
                                                                pLang,
                                                                pTextValue});

            if (!_result.Succeed)
            {
                if (_result.Desc.Contains("PRIMARY KEY constraint "))
                {
                    _result.Desc = "Duplicated language ["+item.Lang+"] text!";
                }
            }
            return _result;
        }

        public static Result TranslatorUpdate(TranslatorValue item)
        {
           
            #region SQL
            string sqlCust = @"
                    UPDATE DATA_GLOBALIZATION SET
                                [TextValue] = @TextValue	
                    WHERE tablename = @TableName and 
                            columnname = @ColumnName and 
                            keyField = @KeyField and 
                            Lang=@Lang";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pTableName = new SqlParameter("@TableName", SqlDbType.NVarChar, 50);
            SqlParameter pColumnName = new SqlParameter("@ColumnName", SqlDbType.NVarChar, 50);
            SqlParameter pKeyField = new SqlParameter("@KeyField", SqlDbType.NVarChar, 50);
            SqlParameter pLang = new SqlParameter("@Lang", SqlDbType.NVarChar, 5);
            SqlParameter pTextValue = new SqlParameter("@TextValue", SqlDbType.NVarChar);

            pTableName.Value = item.TableName;
            pColumnName.Value = item.ColumnName;
            pKeyField.Value = item.KeyField;
            pLang.Value = item.Lang;
            pTextValue.Value = item.TextValue;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pTableName,
                                                                    pColumnName,
                                                                    pKeyField,
                                                                    pLang,
                                                                    pTextValue
                                                                    });

            return _result;
        }

        public static Result TranslatorDelete(TranslatorValue item)
        {
            #region SQL
            string sqlCust = @"
                    delete from DATA_GLOBALIZATION
                        where  tablename = @TableName and 
                                columnname = @ColumnName and 
                                keyField = @KeyField and 
                                Lang=@lang;";

            #endregion

            #region SQL Parameter Initiliaze

            SqlParameter pTableName = new SqlParameter("@TableName", SqlDbType.NVarChar, 50);
            SqlParameter pColumnName = new SqlParameter("@ColumnName", SqlDbType.NVarChar, 50);
            SqlParameter pKeyField = new SqlParameter("@KeyField", SqlDbType.NVarChar, 50);
            SqlParameter pLang = new SqlParameter("@Lang", SqlDbType.NVarChar, 5);

            pTableName.Value = item.TableName;
            pColumnName.Value = item.ColumnName;
            pKeyField.Value = item.KeyField;
            pLang.Value = item.Lang;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                     pTableName,
                                                                    pColumnName,
                                                                    pKeyField,
                                                                    pLang,
                                                                    });

            return _result;
        }

    }
}