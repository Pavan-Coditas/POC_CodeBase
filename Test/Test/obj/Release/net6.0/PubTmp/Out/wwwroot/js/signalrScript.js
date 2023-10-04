const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.start().catch(err => console.error(err));
var receiverId = null;
var chatboxIndex = 1;

function openMessageForm(receiverId, receiverName) {
    const chatboxId = `chatbox-${chatboxIndex}`;
    const messageContainerId = `message-container-${chatboxIndex}`;
    const messageInputId = `message-input-${chatboxIndex}`;
    const sendMessageBtnId = `send-message-btn-${chatboxIndex}`;

    // Create the chatbox dynamically
    const chatboxHtml = `
                <div class="chatbox-holder" id="${chatboxId}">
                  <div class="chatbox">
                    <div class="chatbox-top">
                      <div class="chat-partner-name">
                        <span>${receiverName}</span>
                      </div>
                      <div class="chatbox-icons">
                        <a href="javascript:void(0);"><i class="fa fa-minus"></i></a>
                        <a href="javascript:void(0);"><i class="fa fa-close"></i></a>
                      </div>
                    </div>
                    <div class="chat-messages">
                      <div class="message-box-holder" id="${messageContainerId}">
                        <!-- Messages will be dynamically updated -->
                      </div>
                    </div>
                    <div class="chat-input-holder">
                      <input type="text" id="${messageInputId}" style="width: 100%;" />
                      <input type="button" value="Send" class="message-send" id="${sendMessageBtnId}" />
                    </div>
                  </div>
                </div>
              `;

    // Append the chatbox HTML to the container
    $('.container').append(chatboxHtml);

    // Set the position of the chatbox dynamically
    const chatboxes = $('.chatbox-holder');
    const chatboxWidth = 300; // Adjust the width as needed
    const chatboxSpacing = 20; // Adjust the spacing between chatboxes as needed
    const newPosition = (chatboxes.length - 1) * (chatboxWidth + chatboxSpacing);
    $(`#${chatboxId}`).css('left', newPosition);
    $(`#${messageContainerId}`).empty();

    // Attach event handlers for the new chatbox
    $(`#${sendMessageBtnId}`).click(function (e) {
        let message = $(`#${messageInputId}`).val();
        let sender = receiverId;
        $(`#${messageInputId}`).val('');
        appendSentMessage(chatboxId, receiverId, message);
        connection.invoke('SendMessage', sender, message).catch(err => console.error(err));
        e.preventDefault();
    });

    $(`#${chatboxId} .fa-minus`).click(function () {
        $(this).closest('.chatbox').toggleClass('chatbox-min');
    });

    $(`#${chatboxId} .fa-close`).click(function () {
        $(`#${chatboxId}`).remove(); // Remove the chatbox element from the DOM
    });

    chatboxIndex++;
}

$(document).ready(function () {
    setInterval(updateUserList, 2000);
});

connection.on('ReceiveMessage', (sender, senderId, message) => {
    const existingChatbox = $('.chatbox-holder').filter(function () {
        const partnerName = $(this).find('.chat-partner-name span').text();
        return partnerName === sender;
    });

    if (existingChatbox.length > 0) {
        // Chatbox for the sender is already open, append the received message
        const chatboxId = existingChatbox.attr('id');
        appendReceivedMessage(sender, message, chatboxId);
    } else {
        // Chatbox doesn't exist, check if receiver is online
        const receiverOnline = $('.online-user').filter(function () {
            const userName = $(this).text();
            return userName === sender;
        });

        if (receiverOnline.length > 0) {
            // Receiver is online, open a new chatbox and append the received message
            const chatboxId = `chatbox-${chatboxIndex}`;
            openMessageForm(senderId, sender);
            appendReceivedMessage(sender, message, chatboxId);
        } else {
            // Receiver is not online, display a message to the sender
            alert('The receiver is not currently online.');
        }
    }
});

function appendReceivedMessage(sender, message, chatboxId) {
    let messageContainerId = `#${chatboxId} .message-box-holder`;
    let messageBox = $('<div>').addClass('message-box message-partner').text(message);
    let messageSender = $('<div>').addClass('message-sender').text(sender);

    let messageContainer = $(messageContainerId);
    let incomingMessageContainer = $('<div>').attr('id', `incoming-${chatboxId}`);
    incomingMessageContainer.append(messageSender);
    incomingMessageContainer.append(messageBox);
    incomingMessageContainer.append('<br>'); // Add a line break between messages
    messageContainer.append(incomingMessageContainer);

    // Scroll to the bottom of the message container to show the latest message
    messageContainer.scrollTop(messageContainer.prop('scrollHeight'));
}

function appendSentMessage(chatboxId, receiverId, message) {
    let messageBoxHolder = $(`#${chatboxId} .message-box-holder`);

    if (receiverId === null) {
        // Receiver is not online, append a message indicating the user is not online
        let notOnlineMessage = $('<div>').addClass('message-box').text('The receiver is not currently online.');
        messageBoxHolder.append(notOnlineMessage);
        messageBoxHolder.append('<br>'); // Add a line break
    } else {
        let messageBox = $('<div>').addClass('message-box').text(message);

        if (!messageBoxHolder.find('.message-box').last().text().trim().includes(message.trim())) {
            messageBoxHolder.append(messageBox);
            messageBoxHolder.append('<br>'); // Add a line break between messages
        }
    }
    // Scroll to the bottom of the message container to show the latest message
}

connection.on('UserConnected', (connectionId, userName) => {
    updateUserStatus(connectionId, userName, true);
    connectedUsers.push(userName); // Add the connected user to the list
});

connection.on('UserDisconnected', (connectionId) => {
    updateUserStatus(connectionId, '', false);
});

function updateUserList() {
    $.ajax({
        url: '/Chat/GetConnectedUsers', // Replace with your actual API endpoint
        method: 'GET',
        success: function (data) {
            let userListContainer = $('#userList');
            userListContainer.empty();
            data.forEach(function (user) {
                if (user.userName === '@UserName') {
                    return; // Skip logged-in user
                }
                let statusClass = user.isConnected ? 'online-user' : 'offline-user';
                let userElement = $('<span>').addClass(statusClass).text(user.userName);
                userElement.click(function () {
                    openMessageForm(user.connectionId, user.userName);
                });
                userListContainer.append(userElement);
                userListContainer.append('<br>');
            });
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}

function updateUserStatus(connectionId, userName, isConnected) {
    if (isConnected) {
        connectedUsers.push(connectionId);
    } else {
        let index = connectedUsers.indexOf(connectionId);
        if (index !== -1) {
            connectedUsers.splice(index, 1);
        }
    }

    let userListContainer = $('#userList');
    let userElements = userListContainer.find('span');
    userElements.each(function () {
        let userElement = $(this);
        if (userElement.text() === userName) {
            if (isConnected) {
                userElement.addClass('online-user');
                userElement.removeClass('offline-user');
            } else {
                userElement.addClass('offline-user');
                userElement.removeClass('online-user');
            }
        }
    });
}