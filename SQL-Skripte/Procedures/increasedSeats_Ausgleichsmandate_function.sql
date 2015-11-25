use ElectionDB
GO

--drop function Divisor;
--GO
alter  FUNCTION IncreasingSeats_Ausgleichsmandate(@Election_ID int) RETURNS @tempSeats table(Party_Id int, temp_Seats int, minSeats int, ZweitstimmenCount int) AS

/*THIS FUNCTION IS A CLONE | APPLY CHANGES IN THE TEMPLATE TO ALL OTHER DIVISOR FUNCTIONS*/

BEGIN
	--start value: sum up the min amount of seats for every party

	/*	--deklaration already in the function header
		declare @tempSeats table (
			Party_Id int, 
			temp_Seats int, 
			minSeats int,
			ZweitstimmenCount int);*/
		
		INSERT INTO @tempSeats (Party_Id, temp_Seats, minSeats,ZweitstimmenCount)
		SELECT msppB.Party_Id, 0, msppB.minSeatsBund, (select sum(zsA.Amount)
													from ZweitstimmeAmount zsA
													where zsA.Election_Id=@Election_ID 
													and zsa.Party_Id = msppB.Party_Id)
		from minSeatsPerParty_Bundeslevel msppB
		where msppB.Election_Id=@Election_ID


		declare @MinSeats int = (Select Sum(minSeats)
							from @tempSeats);

		declare @SumPopulation int = (select sum(poB.Count)
								from PopulationBundesland poB
								where poB.Election_Id=@Election_ID);
								 
		declare @flag bit = 0;

		

	while @flag=0
	begin
		if exists (select Party_Id from @tempSeats t
					where t.temp_Seats<t.minSeats) 
			begin
				set @MinSeats = @MinSeats +1; --if there are to less seats dirtibuted to the parties, increase number of seats and update seat amount 

				update @tempSeats
				set temp_Seats =  round( 1.0*ZweitstimmenCount/(1.0*@SumPopulation/@MinSeats),0)

			end --if

		else 
			begin
				set @flag=1
			end
						
	end--while


	return; -- intended: return @tempSeats, but this is handled in the function's header
END--function

