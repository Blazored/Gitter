window.chat = {
    getScrollTop: function (id) {
        const el = document.getElementById(id);
        try {
            if (el) {
                return el.scrollTop;
            } else {
                return -1;
            }
        } catch (e) {
            return -1;
        }
    },
    isScrolledToBottom: function (id) {
        const el = document.getElementById(id);
        try {
            if (el) {
                return el.scrollTop >= el.scrollHeight - el.offsetHeight;
            } else {
                return false;
            }
        } catch (e) {
            return false;
        }
    },
    scrollIntoView: function (id) {
        const el = document.getElementById(id);
        try {
            if (el) {
                el.scrollIntoView();
                return true;
            }
            return false;
        } catch (e) {
            return false;
        }
    },
    preventDefault: function (e) {
        e.preventDefault();
    },
    fileDrop: function (e, id) {
        var files = e.dataTransfer.files;
        if (files.length > 0) {
            for (i in files) {
                var item = files[i];
                chat.processfile(item, id);
            }
        }
        return false;
    },
    filePaste: function (e, id) {
        var items = (e.clipboardData || e.originalEvent.clipboardData).items;
        for (i in items) {
            let item = items[i];
            if (item && item.kind === "file") {
                let file = item.getAsFile();
                chat.processfile(file, id);
            }
        }
    },
    processfile: function (item, id) {
        if (item && item.type && item.type.startsWith('image/')) {
            const elInput = document.getElementById(id);
            var reader = new FileReader();
            reader.addEventListener('load', function () {
                elInput.value = this.result;
                elInput.dispatchEvent(new Event('change'));
            });
            reader.readAsDataURL(item);
        }
    }
};

