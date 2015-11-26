USE [ElectionDB]
GO

/****** Object:  UserDefinedFunction [dbo].[DivisorParty_test]    Script Date: 26.11.2015 17:02:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






--drop function Divisor;
--GO
ALTER  FUNCTION [dbo].[DivisorParty_test](@Election_ID int, @Party_ID int, @Seats int) RETURNS int AS

/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN
	/*rausgezogen, um wiederholtes Auswerten zu vermeiden
	declare @Seats int = (Select s.SeatsParty 
							from Sitzverteilung s
							where s.Election_Id = @Election_ID
							and s.Party_Id= @Party_ID); */
	 
	declare @SumOfSeats int;
	declare @TempDivisor int;

	--initial value of Divisor by naive Divison: Population/available_seats
	set @TempDivisor = (Select Sum(z.Amount) /@Seats
						from ZweitstimmeAmount z
						where z.Election_Id=@Election_ID
						and z.Party_Id=@Party_ID);


	--temporary seats for each state added up for later comparison with actual amount of seats for this party
	/*this is a clone, apply changes also to the computation in the while-loop*/

	set @SumOfSeats = (select Sum(temp.seats)
						from (select votes.Bundesland_Id as b,round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Party_ID= @Party_ID
								) temp
								group by b
								);

	while @SumOfSeats != @Seats
	begin
		if @SumOfSeats > @Seats
		begin
			set @TempDivisor = @TempDivisor + 2500; --if there are to many seats given to the states, the divisor has to be incremented in order to reduce to quotient (which are the single seats) 	
		end --if
		else
		begin
			set @TempDivisor = @TempDivisor *0.9 ; --above comment ^^ vice versa	
		end --else

		--repeat computation of seats for every state with new temporary Divisor
		/*this is a clone from above*/
	set @SumOfSeats = (select Sum(temp.seats)
						from (select votes.Bundesland_Id as b, round(1.0* votes.Amount/@TempDivisor, 0) as seats
								from ZweitstimmenState votes 
								where votes.Election_Id= @Election_ID
								and votes.Party_ID= @Party_ID
								) temp
								group by b
								);
					
	end--while

	return @TempDivisor;
END;--function





GO


