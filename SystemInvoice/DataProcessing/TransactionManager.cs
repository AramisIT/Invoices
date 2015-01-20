using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aramis.DatabaseConnector;
using AramisInfostructure.Queries;

namespace SystemInvoice.DataProcessing
    {
    /// <summary>
    /// Управляет глобальными блокировками на автоматическую генерацию элементов на уровне системы, используется только в классах осуществляющих
    /// загрузку файлов, которая сопровождается считыванием существующих и автоматическим созданием новых элементов справочников.
    /// </summary>
    public class TransactionManager
        {

        private const string GET_TRANSACTION_TEXT = @"--Declare @criticalTime Datetime = current_timestamp;--'20130322';--

if(OBJECT_ID('transactionsSysInvoice','U') is null)
begin
	create table transactionsSysInvoice(ID int identity primary key,TransactionId uniqueidentifier default NewID(),CreationTime Datetime default Current_timestamp);
end;
DECLARE @tempTable table( tranId uniqueidentifier);
Declare @result uniqueidentifier;
Declare @activeCount int = (select Count(*) from transactionsSysInvoice where CreationTime > @criticalTime);
if @activeCount = 0
begin
	delete from transactionsSysInvoice;
	insert transactionsSysInvoice(CreationTime) output inserted.TransactionId into @tempTable   values (CURRENT_TIMESTAMP) ;
	set @result = (select top 1 tranId from @tempTable);
end
select @result;";

        private const string CHECK_TRANSACTION_TEXT = @"--Declare @tranID uniqueidentifier = '41E7D9D7-8F72-4AF7-8B81-96319DD9114C';
if(OBJECT_ID('transactionsSysInvoice','U') is null)
begin
	create table transactionsSysInvoice(ID int identity primary key,TransactionId uniqueidentifier default NewID(),CreationTime Datetime default Current_timestamp);
end;
select COUNT(*) from transactionsSysInvoice where TransactionId = @tranID;";

        private const string COMPLETE_TRANSACTION_TEXT = @"--Declare @tranID uniqueidentifier = '41E7D9D7-8F72-4AF7-8B81-96319DD9114C';
if(OBJECT_ID('transactionsSysInvoice','U') is null)
begin
	create table transactionsSysInvoice(ID int identity primary key,TransactionId uniqueidentifier default NewID(),CreationTime Datetime default Current_timestamp);
end;
delete from transactionsSysInvoice where TransactionId = @tranID;";

        private readonly TimeSpan TRANSACTION_MAX_ALIVE_PERIOD = new TimeSpan( 0, 0, 0, 30, 0 );

        private readonly TimeSpan MAX_WHAITING_FOR_GETTING_TRAN_PERIOD = new TimeSpan( 0, 0, 1, 0, 0 );

        private static TransactionManager transactionManager = new TransactionManager();

        public static TransactionManager TransactionManagerInstance
            {
            get
                {
                return transactionManager;
                }
            }

        private readonly object locker = new object();
        private Guid tranID = new Guid();


        /// <summary>
        /// Используется для явной блокировки автоматической обработки документов для всех приложений подключенных к той же БД для того,
        /// что бы не допустить автоматического создания одинаковых элементов справочников
        /// разными пользователями в процессе обработки файлов. Для завершения блокировки необходимо вызвать CompleteBusingessTransaction, либо
        /// блокировка будет снята автоматически по истечению определенного срока.
        /// </summary>
        public bool BeginBusinessTransaction()
            {
           // return true;
            lock (locker)
                {
                DateTime beginTryGetTranTime = DateTime.Now;
                while (true)
                    {
                    DateTime criticalTime = DateTime.Now - TRANSACTION_MAX_ALIVE_PERIOD;
                    IQuery query = DB.NewQuery( GET_TRANSACTION_TEXT );
                    query.AddInputParameter( "criticalTime", criticalTime );
                    object result = query.SelectScalar();
                    if (result != null && result != DBNull.Value && result is Guid)
                        {
                      //  Console.WriteLine( "tran begin" );
                        tranID = (Guid)result;
                        return true;
                        }
                    if ((DateTime.Now - beginTryGetTranTime) > MAX_WHAITING_FOR_GETTING_TRAN_PERIOD)
                        {
                        "Справочники заблокированны другим пользователем, повторите попытку через 1 минуту.".AlertBox();
                        return false;
                        }
                    }
                }
            }
        /// <summary>
        /// Возвращает - является ли приложение в данный момент в режиме транзакции
        /// </summary>
        public bool IsInTransaction()
            {
        //    return true;
            lock (locker)
                {
             //   Console.WriteLine( "get tran state" );
                IQuery query = DB.NewQuery( CHECK_TRANSACTION_TEXT );
                query.AddInputParameter( "tranID", tranID );
                object result = query.SelectScalar();
                if (result != null && result != DBNull.Value && result is Int32)
                    {
                    return (int)result == 1;
                    }
                }
            return false;
            }

        /// <summary>
        /// Снимает блокировку на уровне всех приложений
        /// </summary>
        public void CompleteBusingessTransaction()
            {
          //  return;
            lock (locker)
                {
                IQuery query = DB.NewQuery( COMPLETE_TRANSACTION_TEXT );
                query.AddInputParameter( "tranID", tranID );
                query.SelectScalar();
                tranID = Guid.NewGuid();
              //  Console.WriteLine( "tran complete" );
                }
            }

        public class TransactionNotGetException : Exception
            {
            public TransactionNotGetException( string message )
                : base( message )
                {
                }
            }
        }
    }
