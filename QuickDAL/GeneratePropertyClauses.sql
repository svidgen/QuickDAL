declare @tableName as varchar(100);
set @tableName = 'customeraddressbase';


--
-- quoted fields list
--
--select
--	'"' + column_name + '",' 'fields'
	
--from
--	information_schema.columns

--where
--	table_name = @tableName
	
--order by
--	column_name asc
	
--;


--
-- public properties
-- 
select
	'public ' +
	case data_type
		when 'nvarchar' then 'String'
		when 'varchar' then 'String'
		when 'text' then 'String'
		when 'binary' then 'byte[]'
		when 'bit' then 'Boolean?'
		when 'int' then 'Int32?'
		when 'bigint' then 'Int64?'
		when 'timestamp' then 'byte[]'
		when 'datetime' then 'DateTime'
		when 'smalldatetime' then 'DateTime'
		when 'uniqueidentifier' then 'Guid'
	end +
	' ' + column_name + ' { get; set; }' 'properties'
	
from
	information_schema.columns

where
	table_name = @tableName
	
order by
	column_name asc
	
;


--
-- Maps
--
select
    '{"' + column_name + '", new Reference<' +
		case data_type
			when 'nvarchar' then 'String'
			when 'varchar' then 'String'
			when 'text' then 'String'
			when 'binary' then 'byte[]'
			when 'bit' then 'Boolean?'
			when 'int' then 'Int32?'
			when 'bigint' then 'Int64?'
			when 'timestamp' then 'byte[]'
			when 'datetime' then 'DateTime'
			when 'smalldatetime' then 'DateTime'
			when 'uniqueidentifier' then 'Guid'
		end
	+ '>(() => ' + column_name + ', v => ' + column_name + ' = v)},' maps
	
from
	information_schema.columns

where
	table_name = @tableName
	
order by
	column_name asc
	
;