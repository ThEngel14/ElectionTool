USE [ElectionDB]
GO

/****** Object:  UserDefinedFunction [dbo].[DivisorParty]    Script Date: 28.11.2015 20:24:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER  FUNCTION [dbo].[DivisorParty](@Election_ID int, @Party_ID int, @Seats int) RETURNS int AS

/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN
	 
	declare @SumOfSeats int;
	declare @TempDivisor int;

	--initial value of Divisor by naive Divison: Votes/available_seats
	set @TempDivisor = (Select Sum(z.Amount) /@Seats
						from ZweitstimmeAmount z
						where z.Election_Id=@Election_ID
						and z.Party_Id=@Party_ID);


	--temporary seats for each state added up for later comparison with actual amount of seats for this party
	/*this is a clone, apply changes also to the computation in the while-loop*/

	set @SumOfSeats = (select Sum(temp.seats)
						from (select round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Party_ID= @Party_ID
								) temp);

	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
			set @TempDivisor = @TempDivisor + 251; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
		end --if
		else
		begin
			set @TempDivisor = @TempDivisor *0.45 * 2 ; --above comment ^^ vice versa	
		end --else

		--repeat computation of seats for every state with new temporary Divisor
		/*this is a clone from above*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select --votes.Bundesland_Id as b, 
								round(1.0* votes.Amount/@TempDivisor, 0) as seats
							from ZweitstimmenState votes 
							where votes.Election_Id= @Election_ID
								and votes.Party_ID= @Party_ID
								) temp
								--group by b
								);
					
	end--while

	return @TempDivisor;
END;--function










GO


