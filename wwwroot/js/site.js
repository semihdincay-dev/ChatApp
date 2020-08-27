var createChannelBtn = document.getElementById('create-channel-btn')
var createChannelModal = document.getElementById('create-channel-modal')

createChannelBtn.addEventListener('click', function () {
    createChannelModal.classList.add('active')
})

function closeModal() {
    createChannelModal.classList.remove('active')
}