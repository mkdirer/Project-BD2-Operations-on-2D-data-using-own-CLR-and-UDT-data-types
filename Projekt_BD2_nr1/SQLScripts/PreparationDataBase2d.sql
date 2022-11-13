use newDB2D
go

-- create tabela Point
IF OBJECT_ID('Point') IS NOT NULL
DROP TABLE Point;
GO
create table Point (
    ID int IDENTITY(1,1) PRIMARY KEY, 
    point dbo.Point
);

-- create tabela Polygon
IF OBJECT_ID('Polygon') IS NOT NULL
DROP TABLE Polygon;
GO
create table Polygon (
    ID int IDENTITY(1,1) PRIMARY KEY, 
    polygon dbo.Polygon
);

go

-- help function for changing varargs input parameter
IF OBJECT_ID('dbo.SplitDimensions') IS NOT NULL
DROP FUNCTION dbo.SplitDimensions;
GO
CREATE FUNCTION dbo.SplitDimensions
(
   @Sequence       VARCHAR(MAX),
   @SeparationMark  CHAR(1)
)
RETURNS TABLE
AS
   RETURN 
   (
       SELECT Temp = CONVERT(INT, Temp)
       FROM
       (
           SELECT Temp = x.i.value('(./text())[1]', 'INT')
           FROM
           (
               SELECT [XML] = CONVERT(XML, '<i>' 
                    + REPLACE(@Sequence, @SeparationMark, '</i><i>') 
                    + '</i>').query('.')
           ) AS a
           CROSS APPLY
           [XML].nodes('i') AS x(i)
       ) AS y
       WHERE Temp IS NOT NULL
   );
go

-- function for inserting polygons with Point id
IF OBJECT_ID('dbo.addPolygonUsingPointFromTable') IS NOT NULL
DROP PROCEDURE dbo.addPolygonUsingPointFromTable;
GO
CREATE PROCEDURE dbo.addPolygonUsingPointFromTable
    @Sequence VARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @PointTemp VARCHAR(5000) 

    SELECT @PointTemp = COALESCE(@PointTemp + '/','') + point.ToString() FROM dbo.Point AS zm
        INNER JOIN dbo.SplitDimensions(@Sequence, ',') AS sequence
        ON zm.ID = sequence.Temp;
	
	insert into dbo.Polygon (polygon) values (@PointTemp);
END
GO

--insert date

insert into dbo.Polygon (polygon) values ('0;0/0;2/4;2/4;0');
insert into dbo.Polygon (polygon) values ('-2;2/-2;4/2;4/2;2/4;2/4;-2/2;-2/2;-4/-2;-4/-2;-2/-4;-2/-4;2');
insert into dbo.Polygon (polygon) values ('0;0/0;8/8;8');

insert into dbo.Point (point) values ('0;0');
insert into dbo.Point (point) values ('0;4');
insert into dbo.Point (point) values ('4;4');
insert into dbo.Point (point) values ('4;0');
insert into dbo.Point (point) values ('0;5');
insert into dbo.Point (point) values ('5;0');
insert into dbo.Point (point) values ('6;-3');
insert into dbo.Point (point) values ('-12;14');
insert into dbo.Point (point) values ('16;7');
insert into dbo.Point (point) values ('7;-7');
insert into dbo.Point (point) values ('-5;10');
insert into dbo.Point (point) values ('13;0');
insert into dbo.Point (point) values ('10;5');