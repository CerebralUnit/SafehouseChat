
var emojiRegex = /[\u{1f191}-\u{1f251}]{2}|[\u{1f300}-\u{1f5ff}\u{1f900}-\u{1f9ff}\u{1f600}-\u{1f64f}\u{1f680}-\u{1f6ff}\u{2600}-\u{26ff}\u{2700}-\u{27bf}\u{1f1e6}-\u{1f1ff}\u{1f191}-\u{1f251}\u{1f004}\u{1f0cf}\u{1f170}-\u{1f171}\u{1f17e}-\u{1f17f}\u{1f18e}\u{3030}\u{2b50}\u{2b55}\u{2934}-\u{2935}\u{2b05}-\u{2b07}\u{2b1b}-\u{2b1c}\u{3297}\u{3299}\u{303d}\u{00a9}\u{00ae}\u{2122}\u{23f3}\u{24c2}\u{23e9}-\u{23ef}\u{25b6}\u{23f8}-\u{23fa}\u{200d}]|(?:[\u{00A9}\u{00AE}\u{203C}\u{2049}\u{2122}\u{2139}\u{2194}-\u{2199}\u{21A9}-\u{21AA}\u{231A}-\u{231B}\u{2328}\u{23CF}\u{23E9}-\u{23F3}\u{23F8}-\u{23FA}\u{24C2}\u{25AA}-\u{25AB}\u{25B6}\u{25C0}\u{25FB}-\u{25FE}\u{2600}-\u{2604}\u{260E}\u{2611}\u{2614}-\u{2615}\u{2618}\u{261D}\u{2620}\u{2622}-\u{2623}\u{2626}\u{262A}\u{262E}-\u{262F}\u{2638}-\u{263A}\u{2640}\u{2642}\u{2648}-\u{2653}\u{2660}\u{2663}\u{2665}-\u{2666}\u{2668}\u{267B}\u{267F}\u{2692}-\u{2697}\u{2699}\u{269B}-\u{269C}\u{26A0}-\u{26A1}\u{26AA}-\u{26AB}\u{26B0}-\u{26B1}\u{26BD}-\u{26BE}\u{26C4}-\u{26C5}\u{26C8}\u{26CE}-\u{26CF}\u{26D1}\u{26D3}-\u{26D4}\u{26E9}-\u{26EA}\u{26F0}-\u{26F5}\u{26F7}-\u{26FA}\u{26FD}\u{2702}\u{2705}\u{2708}-\u{270D}\u{270F}\u{2712}\u{2714}\u{2716}\u{271D}\u{2721}\u{2728}\u{2733}-\u{2734}\u{2744}\u{2747}\u{274C}\u{274E}\u{2753}-\u{2755}\u{2757}\u{2763}-\u{2764}\u{2795}-\u{2797}\u{27A1}\u{27B0}\u{27BF}\u{2934}-\u{2935}\u{2B05}-\u{2B07}\u{2B1B}-\u{2B1C}\u{2B50}\u{2B55}\u{3030}\u{303D}\u{3297}\u{3299}]|(?:\u{D83C}[\u{DC04}\u{DCCF}\u{DD70}-\u{DD71}\u{DD7E}-\u{DD7F}\u{DD8E}\u{DD91}-\u{DD9A}\u{DDE6}-\u{DDFF}\u{DE01}-\u{DE02}\u{DE1A}\u{DE2F}\u{DE32}-\u{DE3A}\u{DE50}-\u{DE51}\u{DF00}-\u{DF21}\u{DF24}-\u{DF93}\u{DF96}-\u{DF97}\u{DF99}-\u{DF9B}\u{DF9E}-\u{DFF0}\u{DFF3}-\u{DFF5}\u{DFF7}-\u{DFFF}]|\u{D83D}[\u{DC00}-\u{DCFD}\u{DCFF}-\u{DD3D}\u{DD49}-\u{DD4E}\u{DD50}-\u{DD67}\u{DD6F}-\u{DD70}\u{DD73}-\u{DD7A}\u{DD87}\u{DD8A}-\u{DD8D}\u{DD90}\u{DD95}-\u{DD96}\u{DDA4}-\u{DDA5}\u{DDA8}\u{DDB1}-\u{DDB2}\u{DDBC}\u{DDC2}-\u{DDC4}\u{DDD1}-\u{DDD3}\u{DDDC}-\u{DDDE}\u{DDE1}\u{DDE3}\u{DDE8}\u{DDEF}\u{DDF3}\u{DDFA}-\u{DE4F}\u{DE80}-\u{DEC5}\u{DECB}-\u{DED2}\u{DEE0}-\u{DEE5}\u{DEE9}\u{DEEB}-\u{DEEC}\u{DEF0}\u{DEF3}-\u{DEF6}]|\u{D83E}[\u{DD10}-\u{DD1E}\u{DD20}-\u{DD27}\u{DD30}\u{DD33}-\u{DD3A}\u{DD3C}-\u{DD3E}\u{DD40}-\u{DD45}\u{DD47}-\u{DD4B}\u{DD50}-\u{DD5E}\u{DD80}-\u{DD91}\u{DDC0}]))/ug;

var ESC_MAP = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;'
};
function escapeHTML(s, forAttribute) {
    return s.replace(forAttribute ? /[&<>'"]/g : /[&<>]/g, function (c) {
        return ESC_MAP[c];
    });
}
function addMessages(messages, prepend) {
    archivedLastMessageAuthor = null;
    var html = '';

    for (var i = 0; i < messages.length; i++) {
        html += buildMessageHtml(messages[i]);
    }

    if (!prepend) {
        if (jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").length)
            jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").append(html);
        else
            jQuery(".chat-body").append(html);
    } else {
        if (jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").length)
            jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").prepend(html);
        else
            jQuery(".chat-body").prepend(html);
    }


    if (scrollPos > 90)
        scrollToBottom(0);
}
function addMessage(message, prepend)
{
    replaceSpecialChars(message);


    let div = null
    let formattedTime = '';

    if (moment().diff(message.createdAt, 'days') === 0) {
        formattedTime = moment(message.createdAt).format("LT");
    }
    else {
        formattedTime = moment(message.createdAt).format("M/D/YYYY LT");
    }

    if (message.author.id !== lastMessageAuthor) {
        div = jQuery("<div class='chat-message group-head'></div>");
        if (message.author.profilePicture) {
            div.html(`
                    <div class="avatar"><img src="${message.author.profilePicture}" /></div>
                    <div class="chat-message-content">
                        <a href="#" class="chat-message-author">${message.author.username}</a>
                        <span class="chat-message-date">${formattedTime}</span>
                        <div class="chat-message-message">
                                ${message.text}
                        </div>
                    </div>
                    `);
        }
        else {
            div.html(`
                    <div class="avatar text-avatar">${message.author.username[0]}</div>
                    <div class="chat-message-content">
                        <a href="#" class="chat-message-author">${message.author.username}</a>
                        <span class="chat-message-date">${formattedTime}</span>
                        <div class="chat-message-message">
                                ${message.text}
                        </div>
                    </div>
                    `);
        }
    }
    else {
        div = jQuery("<div class='chat-message group-child'></div>");

        div.html(`
                    <div class="chat-message-content">
                        <span class="chat-message-date">${formattedTime}</span>
                        <div class="chat-message-message">
                                ${message.text}
                        </div>
                    </div>
                    `);


    }

    lastMessageAuthor = message.author.id;



    if (!prepend) {
        if (jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").length)
            jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").append(div);
        else
            jQuery(".chat-body").append(div);
    } else {
        if (jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").length)
            jQuery(".chat-body > .mCustomScrollBox > .mCSB_container").prepend(div);
        else
            jQuery(".chat-body").prepend(div);
    }


    if (message.text.indexOf('<img') === 0 && div.find('img').length) {
        div.find('img').on('load', function () {
            scrollToBottom(500);
        });
    }
    else {
        if (scrollPos > 90) {
            scrollToBottom();
        }
    }
}


function replaceSpecialChars(message) {
    var match = null;
    var images = {};

    do {
        match = emojiRegex.exec(message.text);

        if (match && match[0]) {
            var img = getEmojiImage(match[0]);
            var altTagRegex = /(?:alt=")([a-z_]+)(?:")/ug;

            var altMatches = altTagRegex.exec(img);

            img = img.replace(altMatches[0], `alt=":${altMatches[1]}:"`)

            if (img)
                images[match[0]] = img;

        }

    }
    while (match !== null);

    for (var emoji in images) {
        message.text = message.text.replace(new RegExp(emoji, 'g'), images[emoji])
    }

    return message;
}
function buildMessageHtml(message) {


    replaceSpecialChars(message);


    let div = null
    let formattedTime = '';

    if (moment().diff(message.createdAt, 'days') === 0) {
        formattedTime = moment(message.createdAt).format("LT");
    }
    else {
        formattedTime = moment(message.createdAt).format("M/D/YYYY LT");
    }


    if (message.author.id !== archivedLastMessageAuthor) {
        div = jQuery("<div class='chat-message group-head'></div>");
        if (message.author.profilePicture) {
            div.html(`
                    <div class="avatar"><img src="${message.author.profilePicture}" /></div>
                    <div class="chat-message-content">
                        <a href="#" class="chat-message-author">${message.author.username}</a>
                        <span class="chat-message-date">${formattedTime}</span>
                        <div class="chat-message-message">
                                ${message.text}
                        </div>
                    </div>
                    `);
        }
        else {
            div.html(`
                    <div class="avatar text-avatar">${message.author.username[0]}</div>
                    <div class="chat-message-content">
                        <a href="#" class="chat-message-author">${message.author.username}</a>
                        <span class="chat-message-date">${formattedTime}</span>
                        <div class="chat-message-message">
                                ${message.text}
                        </div>
                    </div>
                    `);
        }
    }
    else {
        div = jQuery("<div class='chat-message group-child'></div>");

        div.html(`
                    <div class="chat-message-content">
                        <span class="chat-message-date">${formattedTime}</span>
                        <div class="chat-message-message">
                                ${message.text}
                        </div>
                    </div>
                    `);


    }

    archivedLastMessageAuthor = message.author.id;
    lastMessageAuthor = archivedLastMessageAuthor;
    return $('<div>').append(div).html();
}
// if User is looking at previous message we should not scroll down -- not yet implemented
function scrollToBottom(){ 
    var element = jQuery(".chat-body.scroll-hijack"); 
    element.mCustomScrollbar("scrollTo", "bottom");  
}
var picker = new EmojiPicker({ iconSize: 32 })

picker.discover();

function getEmojiImage(emoji) {
    if (emoji.match(/[A-Za-z]+/g))
        return null;

    var flagRegex = /[\u{1f191}-\u{1f251}]/ug;
    var matches = emoji.match(flagRegex);

    var unicode = null;
    if (matches && matches.length > 1) {
        var unicode1 = Config.toUnicode(matches[0]);
        var unicode2 = Config.toUnicode(matches[1]);

        if (!Config.emoji_data[unicode2 + '-' + unicode1])
            unicode = unicode1 + '-' + unicode2;
        else
            unicode = unicode2 + '-' + unicode1;

    }
    else {
        unicode = Config.toUnicode(emoji);

    }


    if (Config.emoji_data[unicode] === undefined)
        return '';

    var name = '';

    if (Array.isArray(Config.emoji_data[unicode][3]))
        name = Config.emoji_data[unicode][3][0];
    else
        name = Config.emoji_data[unicode][3]

    var iconInfo = $.emojiarea.icons[`:${name}:`];

    return $.emojiarea.createIcon(iconInfo)
}