--As an authenticated user of the system, I need to be able to send a transfer of a specific amount of TE Bucks to a registered user.
	--I should be able to choose from a list of users to send TE Bucks to.
		--Access list of all usernames from user table (that are also in account table where user id joins?)
		--SELECT username FROM tenmo_user WHERE user_id != @user_id; - @user_id refers to self/current user
	--A transfer should include the User IDs of the from and to users and the amount of TE Bucks.
		-- Access user ids via account ids(in account table) in transfer table. include amount from transfer
		-- SELECT amount, user_id FROM transfer JOIN account ON account_id IN (account_from, account_to) WHERE user_id = @user_id; - @user_id refers to self/current user
	--A Sending Transfer has an initial status of Approved.
		-- Access created transfer (with sending type) at transfer id to assign transfer status id of 'approved'. 
		-- (SELECT transfer_status_desc FROM transfer_status WHERE transfer_status_id = 2); - approved
		-- (SELECT transfer_type_desc FROM transfer_type WHERE transfer_type_id = 2); - send
		-- if transfer type = 2/send, then INSERT INTO transfer (transfer_status_id) VALUES (2) WHERE transfer_id = @transfer_id; - adjust for this transfer
	--The receiver's account balance is increased by the amount of the transfer.
		-- Access balance in account where account_to = account_id and add amount to balance to get new balance
		--INSERT INTO account (balance) VALUES (@(balance + amount)) JOIN transfer ON account_to = account_id  
	--The sender's account balance is decreased by the amount of the transfer.
		-- Access balance in account where account_from = account_id and subtract amount from balance to get new balance
		--INSERT INTO account (balance) VALUES (@(balance - amount)) JOIN transfer ON account_from = account_id 
	--I must not be allowed to send money to myself. (in 1 row, account_from != account_to)
	--I can't send more TE Bucks than I have in my account.
	--I can't send a zero or negative amount. in C# class (transfer) & property (amount): [RANGE(0.01, Double.PositiveInfinity, ErrorMessage = "Amount must be a positive number."
--As an authenticated user of the system, I need to be able to see transfers I have sent or received.
	-- SELECT amount, user_id FROM transfer JOIN account ON account_id IN (account_from, account_to) WHERE user_id = @user_id; - @user_id refers to self/current user
--As an authenticated user of the system, I need to be able to retrieve the details of any transfer based upon the transfer ID.
	-- [HttpGet("transfers/{transferId}")]
	-- SELECT * FROM transfer WHERE transfer_id = @transfer_id;
	SELECT * FROM tenmo_user;
	SELECT * FROM account;
	SELECT * FROM transfer;
	SELECT * FROM transfer_type;
	SELECT * FROM transfer_status;
	INSERT INTO transfer (transfer_status_id, transfer_type_id, account_from, account_to, amount)
	OUTPUT inserted.transfer_id
	VALUES (2, 2, 2002, 2003, 10.00);

	BEGIN TRANSACTION;
    UPDATE account SET balance = ((SELECT balance FROM account JOIN transfer ON account_id = account_from WHERE transfer_id = 3002)
	- (SELECT amount FROM transfer JOIN account ON account.account_id = transfer.account_from WHERE transfer_id = 3002)) WHERE user_id = 2001;
	UPDATE account SET balance = (SELECT balance FROM account JOIN transfer ON account_id = account_to WHERE transfer_id = 3002)
	+ (SELECT amount FROM transfer JOIN account ON account.account_id = transfer.account_to WHERE transfer_id = 3002);
    COMMIT;

	SELECT balance FROM account JOIN transfer ON account_id = 2001 WHERE transfer_id = 3001
	SELECT balance FROM account JOIN transfer ON account_id = 2002 WHERE transfer_id = 3001

	BEGIN TRANSACTION;
UPDATE account SET balance = 989.5 WHERE account_id = 2001;

UPDATE account SET balance = 1010.5 WHERE account_id = 2002; 
COMMIT;

INSERT INTO transfer (transfer_status_id, transfer_type_id, account_to, account_from, amount)
OUTPUT INSERTED.transfer_id 
VALUES (2, 2, 2002, 2001, 20);

SELECT * FROM transfer WHERE transfer_id = 3001;
SELECT * FROM transfer 
JOIN account ON account.account_id IN (account_from, account_to)  -- or creates double (union statement)
JOIN tenmo_user ON account.user_id = tenmo_user.user_id 


SELECT transfer_id, amount FROM transfer
JOIN account ON account.account_id = account_from OR account.account_id = account_to
JOIN tenmo_user ON account.user_id = tenmo_user.user_id 
WHERE username = 'test'
UNION
SELECT username FROM tenmo_user
JOIN account ON account.user_id = tenmo_user.user_id 
JOIN transfer ON account.account_id = account_from
WHERE account_from != account_id & 
ORDER BY transfer_id;

transfer id
to/from
amount

SELECT username, user_id FROM tenmo_user WHERE user_id != 1001

SELECT transfer_id, username, amount FROM transfer 
JOIN account ON account_id = account_from 
JOIN tenmo_user ON account.user_id = tenmo_user.user_id


SELECT transfer_id, username, amount FROM transfer 
JOIN account ON account_id = account_to 
JOIN tenmo_user ON account.user_id = tenmo_user.user_id

DELETE FROM transfer

SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfer JOIN account ON account_id IN (account_from, account_to) WHERE user_id = 1002;

SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfer JOIN account ON account_id IN (account_from, account_to) WHERE user_id = 1003;
SELECT transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount FROM transfer JOIN account ON account_id IN (account_from, account_to) WHERE user_id = 1001;


SELECT transfer_id, transfer_status_id,
	(SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = account_from)) AS sender, 
	(SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = account_to)) AS recipient , 
	amount FROM transfer 
WHERE 
	(SELECT account_id FROM account WHERE user_id = 1002) 
	IN (account_from, account_to);

SELECT transfer_id, 
(SELECT username FROM tenmo_user WHERE user_id = 
(SELECT user_id FROM account WHERE account_id = account_from)) AS sender, 
(SELECT username FROM tenmo_user WHERE user_id = 
(SELECT user_id FROM account WHERE account_id = account_to)) AS recipient, 
amount FROM transfer 
WHERE (SELECT account_id FROM account WHERE user_id = 1001) 
IN(account_from, account_to) AND transfer_id = 3009;


SELECT transfer_id,
(SELECT transfer_status_desc FROM transfer_status WHERE transfer_status_id = 2) AS transfer_status,
(SELECT transfer_type_desc FROM transfer_type WHERE transfer_type_id = 2) AS transfer_type,
(SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = account_from)) AS sender, 
(SELECT username FROM tenmo_user WHERE user_id = (SELECT user_id FROM account WHERE account_id = account_to)) AS recipient, 
amount FROM transfer 
WHERE (SELECT account_id FROM account WHERE user_id = 1001) IN (account_from, account_to);