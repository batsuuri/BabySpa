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
    public class TourDBContext
    {
        static Result _result;

        //********************* Tour ********************//
        #region Tour
        public static Result TourList(TourSearch search)
        {
            
            if (search.SStartDate == null)
            {
                search.SStartDate = DateTime.MinValue;
            }
            if (search.EStartDate == null)
            {
                search.EStartDate = DateTime.MaxValue;
            }
            StringBuilder cond = new StringBuilder();
            if (search.Status != null && search.Status.Length > 0)
            {
                cond.Append(" and t.status in (" + Helper.ArrayToStr(search.Status).Replace(";", ",") + ")");
            }
            if (search.TourID != null && search.TourID != "")
            {
                cond.Append(" and t.tourid like '%" + search.TourID + "%'");
            }
            if (search.TourName != null && search.TourName != "")
            {
                cond.Append(" and (t.TourName like '%" + search.TourName + "%'");
            }
            if (Func.ToInt(search.TourSeason) > 0)
            {
                cond.Append(" and t.TourSeason=" + search.TourSeason.ToString());
            }
            if (search.TourType != null && search.TourType.Length > 0)
            {
                cond.Append(" and t.TourType in (" + Helper.ArrayToStr(search.TourType).Replace(";", ",") + ")");
            }
            if (search.SStartDate != null && search.SStartDate > default(DateTime))
            {
                cond.Append(" and StartDate>='" + Func.ToDateStr(search.SStartDate) + "'");
            }
            if (search.EStartDate != null && search.EStartDate > default(DateTime))
            {
                cond.Append(" and StartDate<='" + Func.ToDateStr(search.EStartDate) + "'");
            }
           
            string sql = cond.ToString();
            if (sql.Length > 0)
            {
                sql = " where " + sql.TrimStart(" and".ToCharArray());
            }
            return TourList(sql);
        }

        public static Result TourList(string condition = "")
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"Select t.* From Tour t ");
            if (condition != "")
            {
                sql.Append(condition);
            }
            sql.Append(" order by TourSeason, Startdate");
            List<BabySpa.Models.Tour> list = new List<BabySpa.Models.Tour>();
            Tour tour;
            DataTable dt = new DataTable();
            try
            {
                _result = Main.DataSetExecute(sql.ToString());

                if (_result.Succeed)
                {

                    dt = ((DataSet)_result.Data).Tables[0];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        tour = new Tour();
                        tour.TourId = Func.ToStr(dt.Rows[i]["TourID"]).Trim();
                        tour.TourSeason = Func.ToInt(dt.Rows[i]["TourSeason"]);
                        tour.TourName = Func.ToStr(dt.Rows[i]["TourName"]).Trim();
                        tour.TourNameShort = Func.ToStr(dt.Rows[i]["TourNameShort"]).Trim();
                        tour.TourType = Func.ToInt(dt.Rows[i]["TourType"]);
                        tour.StartDate = Func.ToDate(dt.Rows[i]["StartDate"]);
                        tour.EndDate = Func.ToDate(dt.Rows[i]["EndDate"]);
                        tour.Duration = Func.ToInt(dt.Rows[i]["Duration"]);
                        tour.NightCount = Func.ToInt(dt.Rows[i]["NightCount"]);
                        tour.DayCount = Func.ToInt(dt.Rows[i]["DayCount"]);
                        tour.Destination = Func.ToStr(dt.Rows[i]["Destination"]);
                        tour.BasicInfo = Func.ToStr(dt.Rows[i]["BasicInfo"]);
                        tour.MaxGroupSize = Func.ToInt(dt.Rows[i]["MaxGroupSize"]);
                        tour.MinGroupSize = Func.ToInt(dt.Rows[i]["MinGroupSize"]);
                        tour.TourCur = Func.ToStr(dt.Rows[i]["TourCur"]);
                        tour.TourPrice = Func.ToDecimal(dt.Rows[i]["TourPrice"]);
                        tour.Deposit = Func.ToDecimal(dt.Rows[i]["Deposit"]);
                        tour.Discount = Func.ToDecimal(dt.Rows[i]["Discount"]);
                        tour.TourPriceMNT = Func.ToDecimal(dt.Rows[i]["TourPriceMNT"]);
                        tour.CurrentEntrantCount = Func.ToInt(dt.Rows[i]["CurrentEntrantCount"]);
                        tour.ShowInHome = Func.ToByte(dt.Rows[i]["ShowInHome"]);
                        tour.CoverPhoto = Func.ToStr(dt.Rows[i]["CoverPhoto"]);
                        tour.CoverText = Func.ToStr(dt.Rows[i]["CoverText"]);
                        tour.Status = Func.ToInt(dt.Rows[i]["Status"]);
                        tour.SmallGroupCount = Func.ToStr(dt.Rows[i]["SmallGroupCount"]);
                        tour.SmallGroupSupplement = Func.ToDecimal(dt.Rows[i]["SmallGroupSupplement"]);
                        tour.SingleSupplement = Func.ToDecimal(dt.Rows[i]["SingleSupplement"]);
                        tour.CoverTextAlign = Func.ToStr(dt.Rows[i]["CoverTextAlign"]);
                        tour.CoverDateTextAlign = Func.ToStr(dt.Rows[i]["CoverDateTextAlign"]);
                        tour.CoverPhotoAlt = Func.ToStr(dt.Rows[i]["CoverPhotoAlt"]);
                        tour.CoverDateText = Func.ToStr(dt.Rows[i]["CoverDateText"]);
                        tour.Category = Func.ToByte(dt.Rows[i]["Category"]);
                        tour.Character = Func.ToStr(dt.Rows[i]["Character"]);
                        //tour.StatusName = Func.ToStr(dt.Rows[i]["StatusName"]);
                        //tour.TypeName = Func.ToStr(dt.Rows[i]["TypeName"]);
                        //tour.CategoryName = Func.ToStr(dt.Rows[i]["CategoryName"]);
                        tour.RangedPrice = Func.ToStr(dt.Rows[i]["RangedPrice"]).Split(';');
                        tour.TourMapUrl = Func.ToStr(dt.Rows[i]["TourMapUrl"]);

                        list.Add(tour);
                    }

                }
                else
                {
                    Main.ErrorLog("Admin Tour-Select", _result.Desc);
                }
            }
            catch (Exception ex)
            {
                Main.ErrorLog("Admin Tour-Select", ex);
            }
            _result.Data = list;
            return _result;
        }
        public static Result ChangeStatus(string id, int status)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE [TOUR] SET
                   [Status] = @Status WHERE TourID= @TourID";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar,50);
            SqlParameter pStatus = new SqlParameter("@Status", SqlDbType.Int);

            pTourID.Value = id;
            pStatus.Value = status;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                        pStatus,
                                                                        pTourID
                                                                       });

            return _result;
        }

        public static Result Save(Tour tour)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE TOUR SET
                               [TourSeason] = @TourSeason
                              ,[TourName] = @TourName
                              ,[TourNameShort] = @TourNameShort
                              ,[StartDate] = @StartDate
                              ,[EndDate] = @EndDate
                              ,[Duration] = @Duration
                              ,[NightCount] = @NightCount
                              ,[DayCount] = @DayCount
                              ,[TourType] = @TourType
                              ,[BasicInfo] = @BasicInfo
                              ,[MaxGroupSize] = @MaxGroupSize
                              ,[MinGroupSize] = @MinGroupSize
                              ,[TourCur] = @TourCur
                              ,[TourPrice] = @TourPrice
                              ,[Deposit] = @Deposit
                              ,[Discount] = @Discount
                              ,[TourPriceMNT] = @TourPriceMNT
                              ,[CurrentEntrantCount] = @CurrentEntrantCount
                              ,[ShowInHome] = @ShowInHome
                              ,[CoverPhoto] = @CoverPhoto
                              ,[CoverText] = @CoverText
                              ,[SmallGroupCount] = @SmallGroupCount
                              ,[SmallGroupSupplement] = @SmallGroupSupplement
                              ,[SingleSupplement] = @SingleSupplement
                              ,[CoverTextAlign] = @CoverTextAlign
                              ,[CoverDateTextAlign] = @CoverDateTextAlign
                              ,[CoverDateText] = @CoverDateText
                              ,[Character] = @Character
                    WHERE TOURID = @TOURID";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pTOURID = new SqlParameter("@TOURID", SqlDbType.NVarChar, 50);
            SqlParameter pTourSeason = new SqlParameter("@TourSeason", SqlDbType.Int);
            SqlParameter pTourName = new SqlParameter("@TourName", SqlDbType.NVarChar, 100);
            SqlParameter pTourNameShort = new SqlParameter("@TourNameShort", SqlDbType.NVarChar, 50);
            SqlParameter pStartDate = new SqlParameter("@StartDate", SqlDbType.DateTime);
            SqlParameter pEndDate = new SqlParameter("@EndDate", SqlDbType.DateTime);
            SqlParameter pDuration = new SqlParameter("@Duration", SqlDbType.Int);
            SqlParameter pNightCount = new SqlParameter("@NightCount", SqlDbType.Int);
            SqlParameter pDayCount = new SqlParameter("@DayCount", SqlDbType.Int);
            SqlParameter pTourType = new SqlParameter("@TourType", SqlDbType.Char, 2);
            SqlParameter pBasicInfo = new SqlParameter("@BasicInfo", SqlDbType.NVarChar, 4000);
            SqlParameter pMaxGroupSize = new SqlParameter("@MaxGroupSize", SqlDbType.Int);
            SqlParameter pMinGroupSize = new SqlParameter("@MinGroupSize", SqlDbType.Int);
            SqlParameter pTourCur = new SqlParameter("@TourCur", SqlDbType.Char, 3);
            SqlParameter pTourPrice = new SqlParameter("@TourPrice", SqlDbType.Decimal);
            SqlParameter pDeposit = new SqlParameter("@Deposit", SqlDbType.Decimal);
            SqlParameter pDiscount = new SqlParameter("@Discount", SqlDbType.Decimal);
            SqlParameter pTourPriceMNT = new SqlParameter("@TourPriceMNT", SqlDbType.Decimal);
            SqlParameter pCurrentEntrantCount = new SqlParameter("@CurrentEntrantCount", SqlDbType.Int);
            SqlParameter pShowInHome = new SqlParameter("@ShowInHome", SqlDbType.Char, 1);
            SqlParameter pCoverPhoto = new SqlParameter("@CoverPhoto", SqlDbType.NVarChar, 250);
            SqlParameter pCoverText = new SqlParameter("@CoverText", SqlDbType.NVarChar, 250);
            SqlParameter pSmallGroupCount = new SqlParameter("@SmallGroupCount", SqlDbType.Int);
            SqlParameter pSmallGroupSupplement = new SqlParameter("@SmallGroupSupplement", SqlDbType.Decimal);
            SqlParameter pSingleSupplement = new SqlParameter("@SingleSupplement", SqlDbType.Decimal);
            SqlParameter pCoverTextAlign = new SqlParameter("@CoverTextAlign", SqlDbType.NVarChar, 50);
            SqlParameter pCoverDateTextAlign = new SqlParameter("@CoverDateTextAlign", SqlDbType.NVarChar, 50);
            SqlParameter pCoverPhotoAlt = new SqlParameter("@CoverPhotoAlt", SqlDbType.NVarChar, 50);
            SqlParameter pCoverDateText = new SqlParameter("@CoverDateText", SqlDbType.NVarChar, 50);
            SqlParameter pCharacter = new SqlParameter("@Character", SqlDbType.NVarChar, 4000);

            pTOURID.Value = tour.TourId;
            pTourSeason.Value = tour.TourSeason;
            pTourName.Value = tour.TourName;
            pTourNameShort.Value = tour.TourNameShort;
            pStartDate.Value = tour.StartDate;
            pEndDate.Value = tour.EndDate;
            pDuration.Value = tour.Duration;
            pNightCount.Value = tour.NightCount;
            pDayCount.Value = tour.DayCount;
            pTourType.Value = tour.TourType;
            pBasicInfo.Value = tour.BasicInfo;
            pMaxGroupSize.Value = tour.MaxGroupSize;
            pMinGroupSize.Value = tour.MinGroupSize;
            pTourCur.Value = tour.TourCur;
            pTourPrice.Value = tour.TourPrice;
            pDeposit.Value = tour.Deposit;
            pDiscount.Value = tour.Discount;
            pTourPriceMNT.Value = tour.TourPriceMNT;
            pCurrentEntrantCount.Value = tour.CurrentEntrantCount;
            pShowInHome.Value = tour.ShowInHome;
            pCoverPhoto.Value = tour.CoverPhoto;
            pCoverText.Value = tour.CoverText;
            pSmallGroupCount.Value = tour.SmallGroupCount;
            pSmallGroupSupplement.Value = tour.SmallGroupSupplement;
            pSingleSupplement.Value = tour.SingleSupplement;
            pCoverTextAlign.Value = tour.CoverTextAlign;
            pCoverDateTextAlign.Value = tour.CoverDateTextAlign;
            pCoverPhotoAlt.Value = tour.CoverPhotoAlt;
            pCoverDateText.Value = tour.CoverDateText;
            pCharacter.Value = tour.Character;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                       pTOURID,
                                                                        pTourSeason,
                                                                        pTourName,
                                                                        pTourNameShort,
                                                                        pStartDate,
                                                                        pEndDate,
                                                                        pDuration,
                                                                        pNightCount,
                                                                        pDayCount,
                                                                        pTourType,
                                                                        pBasicInfo,
                                                                        pMaxGroupSize,
                                                                        pMinGroupSize,
                                                                        pTourCur,
                                                                        pTourPrice,
                                                                        pDeposit,
                                                                        pDiscount,
                                                                        pTourPriceMNT,
                                                                        pCurrentEntrantCount,
                                                                        pShowInHome,
                                                                        pCoverPhoto,
                                                                        pCoverText,
                                                                        pSmallGroupCount,
                                                                        pSmallGroupSupplement,
                                                                        pSingleSupplement,
                                                                        pCoverTextAlign,
                                                                        pCoverDateTextAlign,
                                                                        pCoverPhotoAlt,
                                                                        pCoverDateText,
                                                                        pCharacter,
                                                                        });

            return _result;
        }

        public static Result Delete(string id)
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

        public static Result Copy(string id, int season)
        {
            #region SQL
            string sqlCust = @"begin
                                set @TempID = dbo.NextTourID(@Season, SUBSTRING(@TourID,6,1));
                                insert into tour
                                select     @TempID
                                           ,@Season
                                           ,[TourName]
                                           ,[TourNameShort]
                                           ,[StartDate]
                                           ,[EndDate]
                                           ,[Duration]
                                           ,[NightCount]
                                           ,[DayCount]
                                           ,[Destination]
                                           ,[TourType]
                                           ,[BasicInfo]
                                           ,[MaxGroupSize]
                                           ,[MinGroupSize]
                                           ,[TourCur]
                                           ,[TourPrice]
                                           ,[Deposit]
                                           ,[Discount]
                                           ,[TourPriceMNT]
                                           ,0
                                           ,0
                                           ,''
                                           ,[CoverText]
                                           ,5
                                           ,[SmallGroupCount]
                                           ,[SmallGroupSupplement]
                                           ,[SingleSupplement]
                                           ,[CoverTextAlign]
                                           ,[CoverDateTextAlign]
                                           ,[CoverPhotoAlt]
                                           ,[CoverDateText]
                                           ,[Category]
                                           ,[Character]
                                           ,[RangedPrice]
                                           ,'' from TOUR
                                where TOURID = @TourID;
                                insert into TOUR_Info
                                select 
			                                @TempID
                                           ,[InfoType]
                                           ,[InfoValue]
                                           ,[InfoValue2]
                                           ,[InfoDesc]
                                           ,[InfoOrder]
                                           ,(select MAX(cast(infokey as int))+1 from TOUR_Info)+ROW_NUMBER()over(order by Infotype asc)
                                from TOUR_Info where TOURID = @TourID;
                                insert into dbo.TOUR_Itinerary
                                select 
			                                @TempID
			                                ,(select MAX(cast(ItineraryID as int))+1 from dbo.TOUR_Itinerary)+ROW_NUMBER()over(order by OrderNo asc)
			                                ,[DayNo]
                                           ,[Subject]
                                           ,[Texts]
                                           ,[Accommodation]
                                           ,[Transport]
                                           ,[Distance]
                                           ,[PhotoName]
                                           ,[Hint]
                                           ,[Destination]
                                           ,[Duration]
                                           ,[Lunch]
                                           ,[Breakfast]
                                           ,[Dinner]
                                           ,[Destination2]
                                           ,[Destination3]
                                           ,[Accommodation2]
                                           ,[Accommodation3]
                                           ,[Lunch2]
                                           ,[Breakfast2]
                                           ,[Dinner2]
                                           ,[Lunch3]
                                           ,[Breakfast3]
                                           ,[Dinner3]
                                           ,[Distance2]
                                           ,[Distance3]
                                           ,[Duration2]
                                           ,[Duration3]
                                           ,[OrderNo]
                                from dbo.TOUR_Itinerary where TOURID = @TourID;
                                end;";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar,50);
            SqlParameter pTempID= new SqlParameter("@TempID", SqlDbType.NVarChar,50);
            SqlParameter pSeason = new SqlParameter("@Season", SqlDbType.Int);

            pTourID.Value = id;
            pSeason.Value = season;
            pTempID.Direction = ParameterDirection.Output;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                        pTourID,
                                                                        pSeason,
                                                                        pTempID
                                                                       });
            if (_result.Succeed)
            {
                _result.Data = pTempID.Value;
            }
            return _result;
        }
        #endregion
        #region Itinerary

        public static Result GetInineraryList(string tourid)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * From TOUR_Itinerary
                            where TourID = @TourID order by orderno, dayno;");
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar);
            pTourID.Value = tourid;


            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTourID});
            return _result;
        }
        public static Result GetIninerary(string id)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * From TOUR_Itinerary
                            where ItineraryId = @ItineraryID;");
            SqlParameter pItineraryID = new SqlParameter("@ItineraryID", SqlDbType.NVarChar);
            pItineraryID.Value = id;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pItineraryID});
            return _result;
        }

        public static Result ItinerarySave(Itinerary item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE TOUR_ITINERARY SET
                                [DayNo] = @DayNo,
                                [Subject] = @Subject,
                                [Texts] = @Texts,
                                [Accommodation] = @Accommodation,
                                [Transport] = @Transport,
                                [Distance] = @Distance,
                                [Hint] = @Hint,
                                [Destination] = @Destination,
                                [Duration] = @Duration,
                                [Lunch] = @Lunch,
                                [Breakfast] = @Breakfast,
                                [Dinner] = @Dinner,
                                [Destination2] = @Destination2,
                                [Destination3] = @Destination3,
                                [Accommodation2] = @Accommodation2,
                                [Accommodation3] = @Accommodation3,
                                [Lunch2] = @Lunch2,
                                [Breakfast2] = @Breakfast2,
                                [Dinner2] = @Dinner2,
                                [Lunch3] = @Lunch3,
                                [Breakfast3] = @Breakfast3,
                                [Dinner3] = @Dinner3,
                                [Distance2] = @Distance2,
                                [Distance3] = @Distance3,
                                [Duration2] = @Duration2,
                                [Duration3] = @Duration3,
                                [OrderNo] = @OrderNo
                    WHERE ItineraryID = @ItineraryID";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pItineraryID = new SqlParameter("@ItineraryID", SqlDbType.Int);
            SqlParameter pDayNo = new SqlParameter("@DayNo", SqlDbType.NVarChar, 50);
            SqlParameter pSubject = new SqlParameter("@Subject", SqlDbType.NVarChar, 150);
            SqlParameter pTexts = new SqlParameter("@Texts", SqlDbType.NVarChar, 4000);
            SqlParameter pAccommodation = new SqlParameter("@Accommodation", SqlDbType.Int);
            SqlParameter pTransport = new SqlParameter("@Transport", SqlDbType.Int);
            SqlParameter pDistance = new SqlParameter("@Distance", SqlDbType.NVarChar, 50);
            SqlParameter pHint = new SqlParameter("@Hint", SqlDbType.NVarChar, 200);
            SqlParameter pDestination = new SqlParameter("@Destination", SqlDbType.Int);
            SqlParameter pDuration = new SqlParameter("@Duration", SqlDbType.NVarChar, 50);
            SqlParameter pLunch = new SqlParameter("@Lunch", SqlDbType.Int);
            SqlParameter pBreakfast = new SqlParameter("@Breakfast", SqlDbType.Int);
            SqlParameter pDinner = new SqlParameter("@Dinner", SqlDbType.Int);
            SqlParameter pDestination2 = new SqlParameter("@Destination2", SqlDbType.Int);
            SqlParameter pDestination3 = new SqlParameter("@Destination3", SqlDbType.Int);
            SqlParameter pAccommodation2 = new SqlParameter("@Accommodation2", SqlDbType.Int);
            SqlParameter pAccommodation3 = new SqlParameter("@Accommodation3", SqlDbType.Int);
            SqlParameter pLunch2 = new SqlParameter("@Lunch2", SqlDbType.Int);
            SqlParameter pBreakfast2 = new SqlParameter("@Breakfast2", SqlDbType.Int);
            SqlParameter pDinner2 = new SqlParameter("@Dinner2", SqlDbType.Int);
            SqlParameter pLunch3 = new SqlParameter("@Lunch3", SqlDbType.Int);
            SqlParameter pBreakfast3 = new SqlParameter("@Breakfast3", SqlDbType.Int);
            SqlParameter pDinner3 = new SqlParameter("@Dinner3", SqlDbType.Int);
            SqlParameter pDistance2 = new SqlParameter("@Distance2", SqlDbType.NVarChar, 50);
            SqlParameter pDistance3 = new SqlParameter("@Distance3", SqlDbType.NVarChar, 50);
            SqlParameter pDuration2 = new SqlParameter("@Duration2", SqlDbType.NVarChar, 50);
            SqlParameter pDuration3 = new SqlParameter("@Duration3", SqlDbType.NVarChar, 50);
            SqlParameter pOrderNo = new SqlParameter("@OrderNo", SqlDbType.Int);



            pItineraryID.Value = item.ItineraryID;
            pDayNo.Value = item.DayNo;
            pSubject.Value = item.Subject;
            pTexts.Value = item.Texts;
            pAccommodation.Value = item.Accommodation.dataID;
            pTransport.Value = item.Transport.dataID;
            pDistance.Value = item.Distance;
            pHint.Value = item.Hint;
            pDestination.Value = item.Destination.dataID;
            pDuration.Value = item.Duration;
            pLunch.Value = item.Lunch.dataID;
            pBreakfast.Value = item.BreakFast.dataID;
            pDinner.Value = item.Dinner.dataID;
            pDestination2.Value = item.Destination2.dataID;
            pDestination3.Value = item.Destination3.dataID;
            pAccommodation2.Value = item.Accommodation2.dataID;
            pAccommodation3.Value = item.Accommodation3.dataID;
            pLunch2.Value = item.Lunch2.dataID;
            pBreakfast2.Value = item.BreakFast2.dataID;
            pDinner2.Value = item.Dinner2.dataID;
            pLunch3.Value = item.Lunch3.dataID;
            pBreakfast3.Value = item.BreakFast3.dataID;
            pDinner3.Value = item.Dinner3.dataID;
            pDistance2.Value = item.Distance2;
            pDistance3.Value = item.Distance3;
            pDuration2.Value = item.Duration2;
            pDuration3.Value = item.Duration3;
            pOrderNo.Value = item.OrderNo;




            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pItineraryID,
                                                                    pDayNo,
                                                                    pSubject,
                                                                    pTexts,
                                                                    pAccommodation,
                                                                    pTransport,
                                                                    pDistance,
                                                                    pHint,
                                                                    pDestination,
                                                                    pDuration,
                                                                    pLunch,
                                                                    pBreakfast,
                                                                    pDinner,
                                                                    pDestination2,
                                                                    pDestination3,
                                                                    pAccommodation2,
                                                                    pAccommodation3,
                                                                    pLunch2,
                                                                    pBreakfast2,
                                                                    pDinner2,
                                                                    pLunch3,
                                                                    pBreakfast3,
                                                                    pDinner3,
                                                                    pDistance2,
                                                                    pDistance3,
                                                                    pDuration2,
                                                                    pDuration3,
                                                                    pOrderNo,
                                                                        });

            return _result;
        }

        public static Result ItineraryInsert(Itinerary item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @ItineraryID= COALESCE(MAX(ItineraryID),0)+1 from TOUR_Itinerary;
                    INSERT INTO [TOUR_Itinerary]
                                       ([TourID]
                                       ,[ItineraryID]
                                       ,[DayNo]
                                       ,[Subject]
                                       ,[Texts]
                                       ,[Accommodation]
                                       ,[Transport]
                                       ,[Distance]
                                       ,[Hint]
                                       ,[Destination]
                                       ,[Duration]
                                       ,[Lunch]
                                       ,[Breakfast]
                                       ,[Dinner]
                                       ,[Destination2]
                                       ,[Destination3]
                                       ,[Accommodation2]
                                       ,[Accommodation3]
                                       ,[Lunch2]
                                       ,[Breakfast2]
                                       ,[Dinner2]
                                       ,[Lunch3]
                                       ,[Breakfast3]
                                       ,[Dinner3]
                                       ,[Distance2]
                                       ,[Distance3]
                                       ,[Duration2]
                                       ,[Duration3]
                                       ,[OrderNo])
                                 VALUES
                                       (@TourID,
                                        @ItineraryID,
                                        @DayNo,
                                        @Subject,
                                        @Texts,
                                        @Accommodation,
                                        @Transport,
                                        @Distance,
                                        @Hint,
                                        @Destination,
                                        @Duration,
                                        @Lunch,
                                        @Breakfast,
                                        @Dinner,
                                        @Destination2,
                                        @Destination3,
                                        @Accommodation2,
                                        @Accommodation3,
                                        @Lunch2,
                                        @Breakfast2,
                                        @Dinner2,
                                        @Lunch3,
                                        @Breakfast3,
                                        @Dinner3,
                                        @Distance2,
                                        @Distance3,
                                        @Duration2,
                                        @Duration3,
                                        @OrderNo
                                        ); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar, 50);
            SqlParameter pItineraryID = new SqlParameter("@ItineraryID", SqlDbType.Int);
            SqlParameter pDayNo = new SqlParameter("@DayNo", SqlDbType.NVarChar, 50);
            SqlParameter pSubject = new SqlParameter("@Subject", SqlDbType.NVarChar, 150);
            SqlParameter pTexts = new SqlParameter("@Texts", SqlDbType.NVarChar, 4000);
            SqlParameter pAccommodation = new SqlParameter("@Accommodation", SqlDbType.Int);
            SqlParameter pTransport = new SqlParameter("@Transport", SqlDbType.Int);
            SqlParameter pDistance = new SqlParameter("@Distance", SqlDbType.NVarChar, 50);
            SqlParameter pHint = new SqlParameter("@Hint", SqlDbType.NVarChar, 200);
            SqlParameter pDestination = new SqlParameter("@Destination", SqlDbType.Int);
            SqlParameter pDuration = new SqlParameter("@Duration", SqlDbType.NVarChar, 50);
            SqlParameter pLunch = new SqlParameter("@Lunch", SqlDbType.Int);
            SqlParameter pBreakfast = new SqlParameter("@Breakfast", SqlDbType.Int);
            SqlParameter pDinner = new SqlParameter("@Dinner", SqlDbType.Int);
            SqlParameter pDestination2 = new SqlParameter("@Destination2", SqlDbType.Int);
            SqlParameter pDestination3 = new SqlParameter("@Destination3", SqlDbType.Int);
            SqlParameter pAccommodation2 = new SqlParameter("@Accommodation2", SqlDbType.Int);
            SqlParameter pAccommodation3 = new SqlParameter("@Accommodation3", SqlDbType.Int);
            SqlParameter pLunch2 = new SqlParameter("@Lunch2", SqlDbType.Int);
            SqlParameter pBreakfast2 = new SqlParameter("@Breakfast2", SqlDbType.Int);
            SqlParameter pDinner2 = new SqlParameter("@Dinner2", SqlDbType.Int);
            SqlParameter pLunch3 = new SqlParameter("@Lunch3", SqlDbType.Int);
            SqlParameter pBreakfast3 = new SqlParameter("@Breakfast3", SqlDbType.Int);
            SqlParameter pDinner3 = new SqlParameter("@Dinner3", SqlDbType.Int);
            SqlParameter pDistance2 = new SqlParameter("@Distance2", SqlDbType.NVarChar, 50);
            SqlParameter pDistance3 = new SqlParameter("@Distance3", SqlDbType.NVarChar, 50);
            SqlParameter pDuration2 = new SqlParameter("@Duration2", SqlDbType.NVarChar, 50);
            SqlParameter pDuration3 = new SqlParameter("@Duration3", SqlDbType.NVarChar, 50);
            SqlParameter pOrderNo = new SqlParameter("@OrderNo", SqlDbType.Int);

            pItineraryID.Direction = ParameterDirection.Output;

            pTourID.Value = item.TourId;
            pItineraryID.Value = item.ItineraryID;
            pDayNo.Value = item.DayNo;
            pSubject.Value = item.Subject;
            pTexts.Value = item.Texts;
            pAccommodation.Value = item.Accommodation.dataID;
            pTransport.Value = item.Transport.dataID;
            pDistance.Value = item.Distance;
            pHint.Value = item.Hint;
            pDestination.Value = item.Destination.dataID;
            pDuration.Value = item.Duration;
            pLunch.Value = item.Lunch.dataID;
            pBreakfast.Value = item.BreakFast.dataID;
            pDinner.Value = item.Dinner.dataID;
            pDestination2.Value = item.Destination2.dataID;
            pDestination3.Value = item.Destination3.dataID;
            pAccommodation2.Value = item.Accommodation2.dataID;
            pAccommodation3.Value = item.Accommodation3.dataID;
            pLunch2.Value = item.Lunch2.dataID;
            pBreakfast2.Value = item.BreakFast2.dataID;
            pDinner2.Value = item.Dinner2.dataID;
            pLunch3.Value = item.Lunch3.dataID;
            pBreakfast3.Value = item.BreakFast3.dataID;
            pDinner3.Value = item.Dinner3.dataID;
            pDistance2.Value = item.Distance2;
            pDistance3.Value = item.Distance3;
            pDuration2.Value = item.Duration2;
            pDuration3.Value = item.Duration3;
            pOrderNo.Value = item.OrderNo;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pTourID,
                                                                    pItineraryID,
                                                                    pDayNo,
                                                                    pSubject,
                                                                    pTexts,
                                                                    pAccommodation,
                                                                    pTransport,
                                                                    pDistance,
                                                                    pHint,
                                                                    pDestination,
                                                                    pDuration,
                                                                    pLunch,
                                                                    pBreakfast,
                                                                    pDinner,
                                                                    pDestination2,
                                                                    pDestination3,
                                                                    pAccommodation2,
                                                                    pAccommodation3,
                                                                    pLunch2,
                                                                    pBreakfast2,
                                                                    pDinner2,
                                                                    pLunch3,
                                                                    pBreakfast3,
                                                                    pDinner3,
                                                                    pDistance2,
                                                                    pDistance3,
                                                                    pDuration2,
                                                                    pDuration3,
                                                                    pOrderNo,
                                                                        });

            if (_result.Succeed)
            {
                _result.Data = pItineraryID.Value;
            }
            return _result;
        }

        public static Result DeleteItinerary(int id)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"Delete from Tour_Itinerary where ItineraryID = @ItineraryID;");
            SqlParameter pItineraryID = new SqlParameter("@ItineraryID", SqlDbType.Int);
            pItineraryID.Value = id;

            _result = Main.ExecuteNonQuery(sql.ToString(), new SqlParameter[] {
                                                                        pItineraryID});
            return _result;
        }

        #endregion

        //******************* Info ***************//
        public static Result GetInfoList(string tourid, string type)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from tour_info
                            where TourID = @TourID and InfoType = @InfoType
                            order by InfoOrder, InfoValue;");
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar);
            SqlParameter pInfoType = new SqlParameter("@InfoType", SqlDbType.NVarChar);
            pTourID.Value = tourid;
            pInfoType.Value = type;


            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTourID,
                                                                       pInfoType });
            return _result;
        }

        public static Result GetInfoDetail(string infoKey)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from tour_info
                            where InfoKey = @InfoKey");
            SqlParameter pInfoKey = new SqlParameter("@InfoKey", SqlDbType.NVarChar);
            pInfoKey.Value = infoKey;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pInfoKey});
            return _result;
        }
        public static Result InfoInsert(Info item)
        {
            if (Func.ToStr(item.InfoType) == "")
            {
                item.InfoType = "-";
            }
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @InfoKey= COALESCE(MAX(cast(InfoKey as int)),0)+1 from TOUR_Info;
                    INSERT INTO [TOUR_Info]
                                       ([TourID]
                                       ,[InfoType]
                                       ,[InfoValue]
                                       ,[InfoValue2]
                                       ,[InfoDesc]
                                       ,[InfoOrder]
                                       ,[InfoKey])
                                 VALUES
                                       (@TourID,
                                        @InfoType,
                                        @InfoValue,
                                        @InfoValue2,
                                        @InfoDesc,
                                        @InfoOrder,
                                        @InfoKey); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Order Parameters
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar, 16);
            SqlParameter pInfoType = new SqlParameter("@InfoType", SqlDbType.NVarChar, 20);
            SqlParameter pInfoValue = new SqlParameter("@InfoValue", SqlDbType.NVarChar, 4000);
            SqlParameter pInfoValue2 = new SqlParameter("@InfoValue2", SqlDbType.NVarChar, 4000);
            SqlParameter pInfoDesc = new SqlParameter("@InfoDesc", SqlDbType.NVarChar, 4000);
            SqlParameter pInfoOrder = new SqlParameter("@InfoOrder", SqlDbType.Int);
            SqlParameter pInfoKey = new SqlParameter("@InfoKey", SqlDbType.NVarChar, 20);


            pInfoKey.Direction = ParameterDirection.Output;

            pTourID.Value = item.TourID;
            pInfoType.Value = item.InfoType;
            pInfoValue.Value = item.InfoValue;
            pInfoValue2.Value = item.InfoValue2;
            pInfoDesc.Value = item.InfoDesc;
            pInfoOrder.Value = item.InfoOrder;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pTourID,
                                                                    pInfoType,
                                                                    pInfoValue,
                                                                    pInfoValue2,
                                                                    pInfoDesc,
                                                                    pInfoOrder,
                                                                    pInfoKey});
            if (_result.Succeed)
            {
                _result.Data = pInfoKey.Value;
            }
            return _result;
        }

        public static Result InfoUpdate(Info item)
        {
            if (Func.ToStr(item.InfoType) == "")
            {
                item.InfoType = "-";
            }
            #region SQL
            string sqlCust = @"
                    
                    UPDATE Tour_Info SET
                                [InfoValue] = @InfoValue,	
                                [InfoValue2] = @InfoValue2,	
                                [InfoDesc] = @InfoDesc,	
                                [InfoOrder] = @InfoOrder
                    WHERE InfoKey = @InfoKey";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pInfoValue = new SqlParameter("@InfoValue", SqlDbType.NVarChar, 4000);
            SqlParameter pInfoValue2 = new SqlParameter("@InfoValue2", SqlDbType.NVarChar, 4000);
            SqlParameter pInfoDesc = new SqlParameter("@InfoDesc", SqlDbType.NVarChar, 4000);
            SqlParameter pInfoOrder = new SqlParameter("@InfoOrder", SqlDbType.Int);
            SqlParameter pInfoKey = new SqlParameter("@InfoKey", SqlDbType.NVarChar, 20);

            pInfoValue.Value = item.InfoValue;
            pInfoValue2.Value = item.InfoValue2;
            pInfoDesc.Value = item.InfoDesc;
            pInfoOrder.Value = item.InfoOrder;
            pInfoKey.Value = item.InfoKey;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pInfoValue,
                                                                    pInfoValue2,
                                                                    pInfoDesc,
                                                                    pInfoOrder,
                                                                    pInfoKey,
                                                                    });

            return _result;
        }

        public static Result InfoDelete(Info item)
        {
            #region SQL
            string sqlCust = @"
                    Delete from Tour_Info 
                        WHERE InfoKey = @InfoKey;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'tour_info'  and ColumnName='infovalue' and KeyField = @InfoKey;";

            #endregion

            #region SQL Parameter Initiliaze
   
            SqlParameter pInfoKey = new SqlParameter("@InfoKey", SqlDbType.NVarChar, 20);

            pInfoKey.Value = item.InfoKey;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pInfoKey
                                                                    });

            return _result;
        }

        public static Result GetOtherDetails(string tourID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@" begin
                            declare @cnt int;
                            select @cnt =COUNT(Tourid) from TOUR_Info
                            where TourID =@TourID  and InfoType = 'included'
                            if(@cnt =0)
                            begin
                            insert into TOUR_Info
                            values(@TourID,'included','-','','',0,(select MAX(cast(infokey as int))+1 from TOUR_Info));
                            insert into TOUR_Info
                            values(@TourID,'notincluded','-','','',0,(select MAX(cast(infokey as int))+1 from TOUR_Info));
                            insert into TOUR_Info
                            values(@TourID,'accommodation','-','','',0,(select MAX(cast(infokey as int))+1 from TOUR_Info));
                            insert into TOUR_Info
                            values(@TourID,'package','-','','',0,(select MAX(cast(infokey as int))+1 from TOUR_Info));
                            insert into TOUR_Info
                            values(@TourID,'googlemapurl','-','','',0,(select MAX(cast(infokey as int))+1 from TOUR_Info));
                            end;
                           select
                              max(case when InfoType = 'included' then InfoValue end) include,
                              max(case when InfoType = 'notincluded' then InfoValue end) notinclude,
                              max(case when InfoType = 'accommodation' then InfoValue end) accommodation,
                              max(case when InfoType = 'package' then InfoValue end) package,
                              max(case when InfoType = 'googlemapurl' then InfoValue end) googlemapurl,
                              max(RangedPrice) RangedPrice,
                              max(TourMapUrl) TourMapUrl
                        from TOUR_Info i
                        left join TOUR t on i.TourID = t.TOURID
                        where i.TOURID = @TourID; end;");
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar);
            pTourID.Value = tourID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTourID});
            return _result;
        }

        public static Result DeteilsUpdate(TourDetailViewModel item)
        {
            #region SQL
            string sqlCust = @"BEGIN
                    UPDATE Tour_Info SET
                               [InfoValue] = 
                                        case infotype 
                                            when 'included' then @Include 
                                            when 'notincluded' then @NotInclude 
                                            when 'package' then @Package 
                                            when 'accommodation' then @Accommodation 
                                            when 'googlemapurl' then @GoogleMapUrl 
                                            else [InfoValue] 
                                        end
                    WHERE TourID = @TourID;
                    UPDATE TOUR SET 
                        RANGEDPRICE = @RangedPrice, 
                        TourMapUrl = @TourMapUrl 
                    WHERE TourID = @TourID;END;";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pInclude = new SqlParameter("@Include", SqlDbType.NVarChar, 4000);
            SqlParameter pNotInclude = new SqlParameter("@NotInclude", SqlDbType.NVarChar, 4000);
            SqlParameter pPackage = new SqlParameter("@Package", SqlDbType.NVarChar, 4000);
            SqlParameter pAccommodation = new SqlParameter("@Accommodation", SqlDbType.NVarChar, 200);
            SqlParameter pRangedPrice = new SqlParameter("@RangedPrice", SqlDbType.NVarChar, 250);
            SqlParameter pTourMapUrl = new SqlParameter("@TourMapUrl", SqlDbType.NVarChar, 250);
            SqlParameter pGoogleMapUrl = new SqlParameter("@GoogleMapUrl", SqlDbType.NVarChar, 500);
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar, 50);

            pInclude.Value = item.Included;
            pNotInclude.Value = item.NotIncluded;
            pPackage.Value = item.Package;
            pAccommodation.Value = item.Accommodation;
            pRangedPrice.Value = item.RangedPrice;
            pTourMapUrl.Value = item.MapPath;
            pTourID.Value = item.TourID;
            pGoogleMapUrl.Value = item.GoogleMapUrl;
            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                    pInclude,
                                                                    pNotInclude,
                                                                    pPackage,
                                                                    pAccommodation,
                                                                    pRangedPrice,
                                                                    pTourMapUrl,
                                                                    pTourID,
                                                                    pGoogleMapUrl,
                                                                    });

            return _result;
        }

        // ********************* Photos *******************//

        public static Result GetPhotoList(string tourid)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from tour_photo
                            where TourID = @TourID
                            order by DayNo, PhotoName;");
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar);
            pTourID.Value = tourid;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pTourID,});
            return _result;
        }

        public static Result GetPhotoDetail(int photoID)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from tour_photo
                            where photoid= @PhotoID");
            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);
            pPhotoID.Value = photoID;

            _result = Main.DataSetExecute(sql.ToString(), new SqlParameter[] {
                                                                        pPhotoID});
            return _result;
        }
        public static Result PhotoInsert(TourPhoto item)
        {
            #region SQL
            string sqlCust = @"
                    BEGIN 
                    SELECT @PhotoID= COALESCE(MAX(PhotoID),0)+1 from TOUR_Photo;
                        INSERT INTO [TOUR_Photo]
                                   ([TourID]
                                   ,[PhotoName]
                                   ,[PhotoUrl]
                                   ,[DayNo]
                                   ,[PhotoDesc]
                                   ,[PhotoID])
                                 VALUES
                                       (@TourID,
                                        @PhotoName,
                                        @PhotoUrl,
                                        @DayNo,
                                        @PhotoDesc,
                                        @PhotoID); END;";

            #endregion

            #region SQL Parameter Initiliaze
            // Photo Parameters
            SqlParameter pTourID = new SqlParameter("@TourID", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoName = new SqlParameter("@PhotoName", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pDayNo = new SqlParameter("@DayNo", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoDesc = new SqlParameter("@PhotoDesc", SqlDbType.NVarChar, 150);
            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);


            pPhotoID.Direction = ParameterDirection.Output;

            pTourID.Value = item.TourID;
            pPhotoName.Value = item.PhotoName;
            pPhotoUrl.Value = item.PhotoUrl;
            pDayNo.Value = item.DayNo;
            pPhotoDesc.Value = item.PhotoDesc;

            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                   pTourID,
                                                                    pPhotoName,
                                                                    pPhotoUrl,
                                                                    pDayNo,
                                                                    pPhotoDesc,
                                                                    pPhotoID,
                                                                    });
            if (_result.Succeed)
            {
                _result.Data = pPhotoID.Value;
            }
            return _result;
        }

        public static Result PhotoUpdate(TourPhoto item)
        {
            #region SQL
            string sqlCust = @"
                    
                    UPDATE Tour_Photo SET
                                [PhotoName] = @PhotoName,
                                [PhotoUrl] = @PhotoUrl,
                                [DayNo] = @DayNo,
                                [PhotoDesc] = @PhotoDesc
                    WHERE PhotoID = @PhotoID";

            #endregion

            #region SQL Parameter Initiliaze
            SqlParameter pPhotoName = new SqlParameter("@PhotoName", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoUrl = new SqlParameter("@PhotoUrl", SqlDbType.NVarChar, 250);
            SqlParameter pDayNo = new SqlParameter("@DayNo", SqlDbType.NVarChar, 50);
            SqlParameter pPhotoDesc = new SqlParameter("@PhotoDesc", SqlDbType.NVarChar, 150);
            SqlParameter pPhotoID = new SqlParameter("@PhotoID", SqlDbType.Int);


            pPhotoName.Value = item.PhotoName;
            pPhotoUrl.Value = item.PhotoUrl;
            pDayNo.Value = item.DayNo;
            pPhotoDesc.Value = item.PhotoDesc;
            pPhotoID.Value = item.PhotoID;


            #endregion
            _result = Main.ExecuteNonQuery(sqlCust, new SqlParameter[] {
                                                                pPhotoName,
                                                                pPhotoUrl,
                                                                pDayNo,
                                                                pPhotoDesc,
                                                                pPhotoID,
                                                                    });

            return _result;
        }

        public static Result PhotoDelete(TourPhoto item)
        {
            #region SQL
            string sqlCust = @"
                    Delete from Tour_Photo
                        WHERE PhotoID = @PhotoID;
                    delete from DATA_GLOBALIZATION
                        where TableName = 'tour_photo'  and (ColumnName in ('photoname','photodesc' )) and KeyField = @PhotoID;";

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

    }
}