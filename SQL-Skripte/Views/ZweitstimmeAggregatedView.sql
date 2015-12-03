Create View ZweitstimmeAggregated as

/*used for special mode: select data based on single votes: open this view, comment first function out, un-comment second function*/

-- Based on already aggregated table
select Election_Id, Party_Id, Wahlkreis_Id, Amount from ZweitstimmeAmount

-- Based on signle votes
--select Election_Id, Party_Id, Wahlkreis_Id, count(*) from Zweitstimme group by Election_Id, Party_Id, Wahlkreis_Id
