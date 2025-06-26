<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatWindow.aspx.cs" Inherits="ChatWindow" ResponseEncoding="utf-8" Culture="en-US" UICulture="en" %>


<html>
<head runat="server">
    <title>Chat</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <style>
        body {
            background-color: #e5ddd5;
            font-family: 'Segoe UI', sans-serif;
        }

        .chat-container {
            display: flex;
            height: 90vh;
            border-radius: 15px;
            overflow: hidden;
        }

        .sidebar {
            width: 25%;
            background-color: #ffffff;
            border-right: 1px solid #ccc;
            overflow-y: auto;
            padding-top: 10px;
        }

        .user-item {
            padding: 12px 15px;
            border-bottom: 1px solid #f1f1f1;
            cursor: pointer;
            transition: background-color 0.2s;
            font-weight: 500;
            color: #333;
        }

            .user-item:hover,
            .user-item.active {
                background-color: #dcf8c6;
            }

        .chat-area {
            width: 75%;
            display: flex;
            flex-direction: column;
            background-color: #ece5dd;
            position: relative;
        }

        .chat-header {
            background-color: #f0f0f0;
            border-bottom: 1px solid #ccc;
        }

        .chat-messages {
            flex: 1;
            padding: 20px;
            overflow-y: auto;
            background-image: url('https://www.transparenttextures.com/patterns/paper-fibers.png');
            background-repeat: repeat;
        }

        .chat-input {
            padding: 10px 20px;
            background-color: #f0f0f0;
            border-top: 1px solid #ccc;
        }

        .input-group {
            display: flex;
        }

        .form-control {
            flex: 1;
            border: none;
            padding: 12px 15px;
            border-radius: 25px;
            font-size: 14px;
            outline: none;
            background-color: #fff;
            box-shadow: 0 0 3px rgba(0,0,0,0.1) inset;
        }

        .btn-success {
            border: none;
            border-radius: 25px;
            margin-left: 10px;
            padding: 10px 20px;
            background-color: #128c7e;
            color: #fff;
            font-weight: bold;
            transition: background-color 0.3s;
        }

            .btn-success:hover {
                background-color: #075e54;
            }

        /* Optional: message bubble styling */
        .message {
            max-width: 70%;
            padding: 10px 15px;
            border-radius: 15px;
            margin-bottom: 10px;
            font-size: 14px;
            line-height: 1.4;
            clear: both;
            display: inline-block;
        }

            .message.sent {
                background-color: #dcf8c6;
                float: right;
                text-align: right;
            }

            .message.received {
                background-color: #fff;
                float: left;
                text-align: left;
            }

        /* Scrollbars */
        .chat-messages::-webkit-scrollbar,
        .sidebar::-webkit-scrollbar {
            width: 6px;
        }

        .chat-messages::-webkit-scrollbar-thumb,
        .sidebar::-webkit-scrollbar-thumb {
            background-color: #bbb;
            border-radius: 3px;
        }

        #chatBox {
            max-height: 500px; /* example scrollable height */
            overflow-y: auto;
            position: relative;
        }

        #readNowBtn {
            display: none;
            position: absolute;
            bottom: 10px;
            left: 50%;
            transform: translateX(-50%);
            background-color: #007bff;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            cursor: pointer;
            box-shadow: 0 2px 5px rgba(0,0,0,0.2);
            z-index: 10;
        }
    </style>
    <style>
        .chat-message {
            max-width: 70%;
            margin: 5px 0;
            padding: 8px 12px;
            border-radius: 10px;
            clear: both;
            display: inline-block;
            position: relative;
        }

            .chat-message.me {
                background-color: #dcf8c6; /* WhatsApp green */
                float: right;
                text-align: right;
                border-top-right-radius: 0;
            }

            .chat-message.you {
                background-color: #f1f0f0; /* WhatsApp gray */
                float: left;
                text-align: left;
                border-top-left-radius: 0;
            }

        .msg-text {
            font-size: 15px;
            color: #000;
        }

        .msg-time {
            font-size: 10px;
            color: #888;
            margin-top: 4px;
        }


        .whatsapp-back-btn {
            background-color: #25D366; /* WhatsApp green */
            color: #fff;
            padding: 6px 14px;
            border: none;
            border-radius: 20px;
            font-size: 14px;
            text-decoration: none;
            font-weight: 500;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

            .whatsapp-back-btn:hover {
                background-color: #1ebc57; /* slightly darker on hover */
                text-decoration: none;
            }

        #emojiPicker span {
            cursor: pointer;
            font-size: 20px;
            padding: 5px;
            display: inline-block;
        }

            #emojiPicker span:hover {
                background-color: #e0e0e0;
                border-radius: 5px;
            }
    </style>
    <style>
        .user-item {
            padding: 10px;
            cursor: pointer;
            transition: background-color 0.3s ease;
        }

            .user-item:hover {
                background-color: #f0f0f0;
            }

            .user-item.selected {
                background-color: #007bff;
                color: white;
                font-weight: bold;
            }
    </style>
    <style>
        .user-item {
            position: relative;
            padding: 10px;
            cursor: pointer;
        }

            .user-item .badge {
                background-color: red;
                color: white;
                border-radius: 12px;
                padding: 2px 6px;
                font-size: 12px;
                position: absolute;
                top: 10px;
                right: 15px;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />

        <asp:HiddenField ID="hdnSenderID" runat="server" />
        <asp:HiddenField ID="hdnReceiverID" runat="server" />

        <div class="row">
            <!-- Left Section: App Title & Sender Name -->
            <div class="col-md-12">
                <div class="page-header-breadcrumb" style="display: flex; align-items: center; margin-left: 60px;">

                    <span id="selectedSenderName" style="margin-left: 230px; font-size: 20px; color: #555;"></span>
                </div>
            </div>
        </div>

        <div class="chat-container">

            <!-- Sidebar -->
            <div class="sidebar">
                <asp:Repeater ID="rptUsers" runat="server">
                    <ItemTemplate>
                        <div class="user-item" onclick="selectUser('<%# Eval("Userid") %>', this, '<%# Eval("Username") %>')">
                            <span class="username"><%# Eval("Username") %></span>
                            <asp:Literal runat="server"
                                Text='<%# (Convert.ToInt32(Eval("UnreadCount")) > 0) ? 
                                     "<span class=\"badge\">" + Eval("UnreadCount") + "</span>" : "" %>' />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- Chat -->
            <div class="chat-area">
                <div id="chatBox" runat="server" class="chat-messages" style="position: relative;">
                    <button id="readNowBtn">⬇️ Read Now</button>
                </div>
                <div class="chat-input">
                    <div class="input-group">
                        <asp:TextBox ID="txtMessage" TextMode="MultiLine" runat="server" CssClass="form-control" placeholder="Type a message..."></asp:TextBox>
                        <!-- Emoji Button -->
                        <div class="input-group-append">
                            <button type="button" class="btn btn-light" onclick="toggleEmojiPicker()" title="Insert Emoji">😊</button>
                        </div>
                        <button type="button" class="btn btn-success" onclick="sendMessage()">Send</button>
                    </div>

                    <div id="emojiPicker" style="display: none; padding: 5px; background: #f1f1f1; border: 1px solid #ccc; margin-top: 5px; max-height: 200px; overflow-y: auto;">
                        <!-- Smileys & Emotion -->
                        <span>😀</span> <span>😃</span> <span>😄</span> <span>😁</span> <span>😆</span> <span>😂</span> <span>🤣</span>
                        <span>😊</span> <span>😇</span> <span>🙂</span> <span>🙃</span> <span>😉</span> <span>😍</span> <span>😘</span>
                        <span>😗</span> <span>😚</span> <span>😙</span> <span>😋</span> <span>😜</span> <span>😝</span> <span>😛</span>
                        <span>🫠</span> <span>🥰</span> <span>😎</span> <span>🤓</span> <span>😢</span> <span>😭</span> <span>😡</span>

                        <!-- Gestures -->
                        <span>👍</span> <span>👎</span> <span>👏</span> <span>🙌</span> <span>🙏</span> <span>🤝</span> <span>💪</span>

                        <!-- Animals -->
                        <span>🐶</span> <span>🐱</span> <span>🐭</span> <span>🐹</span> <span>🐰</span> <span>🦊</span> <span>🐻</span> <span>🐼</span>

                        <!-- Food -->
                        <span>🍎</span> <span>🍌</span> <span>🍉</span> <span>🍇</span> <span>🍓</span> <span>🍕</span> <span>🍔</span> <span>🍟</span>

                        <!-- Weather & Nature -->
                        <span>☀️</span> <span>🌤️</span> <span>⛅</span> <span>🌧️</span> <span>🌩️</span> <span>❄️</span> <span>🌈</span>

                        <!-- Travel & Places -->
                        <span>🚗</span> <span>✈️</span> <span>🚀</span> <span>🚁</span> <span>🚉</span> <span>🗽</span> <span>🗺️</span>

                        <!-- Activities -->
                        <span>⚽</span> <span>🏀</span> <span>🏈</span> <span>🎾</span> <span>🎲</span> <span>🎮</span> <span>🎸</span>

                        <!-- Objects -->
                        <span>📱</span> <span>💻</span> <span>📷</span> <span>🔔</span> <span>📦</span> <span>📚</span> <span>🔑</span>

                        <!-- Symbols -->
                        <span>❤️</span> <span>💔</span> <span>💯</span> <span>🔥</span> <span>🌟</span> <span>✨</span> <span>⚡</span>
                    </div>


                </div>
            </div>

        </div>
        <script>
            function toggleEmojiPicker() {
                var picker = document.getElementById('emojiPicker');
                picker.style.display = (picker.style.display === 'none') ? 'block' : 'none';
            }

            document.addEventListener("DOMContentLoaded", function () {
                // Add click event to each emoji inside the picker
                document.querySelectorAll("#emojiPicker span").forEach(function (emojiSpan) {
                    emojiSpan.addEventListener("click", function () {
                        var txtBox = document.getElementById('<%= txtMessage.ClientID %>');
                        insertAtCursor(txtBox, emojiSpan.textContent);
                        txtBox.focus();
                    });
                });
            });

            function insertAtCursor(input, emoji) {
                if (document.selection) {
                    // For IE
                    input.focus();
                    var sel = document.selection.createRange();
                    sel.text = emoji;
                } else if (input.selectionStart || input.selectionStart === 0) {
                    // For modern browsers
                    let start = input.selectionStart;
                    let end = input.selectionEnd;
                    let text = input.value;

                    input.value = text.substring(0, start) + emoji + text.substring(end, text.length);
                    input.selectionStart = input.selectionEnd = start + emoji.length;
                } else {
                    input.value += emoji;
                }
            }
        </script>

        <script type="text/javascript">
            $(document).ready(function () {
                var senderId = $('#<%= hdnSenderID.ClientID %>').val();
                var receiverId = $('#<%= hdnReceiverID.ClientID %>').val();

                var chatIntervalId;

                function startChatInterval() {
                    chatIntervalId = setInterval(loadChat1, 500);
                }

                function stopChatIntervalTemporarily(duration) {
                    clearInterval(chatIntervalId);
                    setTimeout(startChatInterval, duration);
                }

                function loadChat1() {
                    var senderId = $('#<%= hdnSenderID.ClientID %>').val();
                    var receiverId = $('#<%= hdnReceiverID.ClientID %>').val();

                    $.ajax({
                        type: "POST",
                        url: "ChatWindow.aspx/LoadChatAjax",
                        data: JSON.stringify({ senderId: senderId, receiverId: receiverId }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            var chatBox = $('#chatBox');
                            var isAtBottom = chatBox[0].scrollHeight - chatBox.scrollTop() <= chatBox.outerHeight() + 50;
                            var oldScrollHeight = chatBox[0].scrollHeight;

                            // Update chat messages
                            chatBox.html(response.d);

                            // Ensure "Read Now" button exists only once
                            if ($('#readNowBtn').length === 0) {
                                chatBox.after('<button id="readNowBtn" style="display: none;">⬇️ New Message</button>');
                            }

                            var newScrollHeight = chatBox[0].scrollHeight;

                            if (isAtBottom) {
                                chatBox.scrollTop(newScrollHeight);
                                $('#readNowBtn').hide();
                            } else if (newScrollHeight > oldScrollHeight) {
                                $('#readNowBtn').show();

                                stopChatIntervalTemporarily(8000);

                                clearTimeout($('#readNowBtn').data('timeoutId'));

                                var timeoutId = setTimeout(function () {
                                    $('#readNowBtn').fadeOut();
                                }, 8000);

                                $('#readNowBtn').data('timeoutId', timeoutId);
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error("Error:", error);
                        }
                    });
                }

                // Read Now scroll
                $(document).on('click', '#readNowBtn', function () {
                    var chatBox = $('#chatBox');
                    chatBox.scrollTop(chatBox[0].scrollHeight);
                    $(this).hide();
                    startChatInterval(); // resume if needed
                });

                // Initial load
                loadChat1();
                startChatInterval();

                // User selection from list
                window.selectUser = function (userId, element, userName) {
                    $('#<%= hdnReceiverID.ClientID %>').val(userId);
                    $('.user-item').removeClass('selected-user');
                    $(element).addClass('selected-user');
                    $('#selectedSenderName').text("🧑 " + userName);
                    loadChat();

                    var items = document.querySelectorAll('.user-item');
                    items.forEach(function (item) {
                        item.classList.remove('selected');
                    });

                    // Add 'selected' class to clicked user
                    element.classList.add('selected');

                    var badge = element.querySelector('.badge');
                    if (badge) badge.style.display = 'none';
                };

                // Send message
                window.sendMessage = function () {
                    var senderId = $('#<%= hdnSenderID.ClientID %>').val();
                    var receiverId = $('#<%= hdnReceiverID.ClientID %>').val();
                    var message = $('#<%= txtMessage.ClientID %>').val();

                    if ($.trim(message) === "") return;

                    PageMethods.SendMessageAjax(senderId, receiverId, message, function () {
                        $('#<%= txtMessage.ClientID %>').val('');
                        loadChat();
                    });
                };

                // Load chat on user switch or after sending
                window.loadChat = function () {
                    var senderId = $('#<%= hdnSenderID.ClientID %>').val();
                    var receiverId = $('#<%= hdnReceiverID.ClientID %>').val();

                    PageMethods.LoadChatAjax(senderId, receiverId, function (response) {
                        $('#<%= chatBox.ClientID %>').html(response);
                        scrollChatToBottom();
                    });
                };

                // Scroll bottom helper
                function scrollChatToBottom() {
                    var chatBox = document.getElementById('<%= chatBox.ClientID %>');
                    chatBox.scrollTop = chatBox.scrollHeight;
                }

                // Send on Enter (no Shift)
                $('#<%= txtMessage.ClientID %>').keypress(function (e) {
                    if (e.which === 13 && !e.shiftKey) {
                        e.preventDefault();
                        sendMessage();
                    }
                });
            });

        </script>
    </form>
</body>
</html>
