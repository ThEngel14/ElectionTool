Create View ErststimmeAggregated as

-- Based on already aggregated table
select Election_Id, Wahlkreis_Id, Person_Id, Amount from ErststimmeAmount

-- Based on single votes
--select Election_Id, Wahlkreis_Id, Person_Id, count(*) from Erststimme group by Election_Id, Wahlkreis_Id, Person_Id