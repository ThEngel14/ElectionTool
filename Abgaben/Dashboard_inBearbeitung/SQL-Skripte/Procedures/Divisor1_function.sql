USE [ElectionDB]
GO

/****** Object:  UserDefinedFunction [dbo].[Divisor]    Script Date: 28.11.2015 20:21:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





--drop function Divisor;
--GO
ALTER  FUNCTION [dbo].[Divisor](@Election_ID int) RETURNS int with schemabinding AS
BEGIN
	declare @Seats int = 598;
	declare @SumOfSeats int;
	declare @TempDivisor int;

	--initial value of Divisor by naive Divison: Population/available_seats
	set @TempDivisor = (Select sum(poB.Count) from dbo.PopulationBundesland poB
				where poB.Election_Id = @Election_ID
			  ) / @Seats;


	--temporary seats of each state, added up for comparison with actual amount of seats
	/*this is a clone*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select round(1.0* poB.Count/@TempDivisor, 0) as seats
								from dbo.PopulationBundesland poB 
								where poB.Election_Id= @Election_ID) temp); 


	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
			set @TempDivisor = @TempDivisor + 3; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
		end --if
		else
		begin
			set @TempDivisor = @TempDivisor -250; --above comment ^^ vice versa	
		end --else

		--repeat computation of seats for every state with new temporary Divisor
		/*this is a clone from above*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select round(1.0* poB.Count/@TempDivisor, 0) as seats
								from dbo.PopulationBundesland poB 
								where poB.Election_Id= @Election_ID) temp
			);
	end--while

	return @TempDivisor;
END;--function




GO


