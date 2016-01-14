USE [ElectionDB]
GO

/****** Object:  View [dbo].[ZweitstimmeAggregated]    Script Date: 03.01.2016 14:00:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[ZweitstimmeAggregated] as

/*used for special mode: select data based on single votes: open this view, comment first function out, un-comment second function*/

-- Based on already aggregated table
select Election_Id, Party_Id, Wahlkreis_Id, Amount from ZweitstimmeAmount

-- Based on single votes
--select Election_Id, Party_Id, Wahlkreis_Id, count(*) from Zweitstimme group by Election_Id, Party_Id, Wahlkreis_Id

GO


