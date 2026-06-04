/* ── Tab payment ── */
function switchPay(btn, id) {
  document
    .querySelectorAll(".pay-tab")
    .forEach((b) => b.classList.remove("active"));
  document
    .querySelectorAll(".pay-content")
    .forEach((c) => c.classList.remove("active"));
  btn.classList.add("active");
  document.getElementById("pay-" + id).classList.add("active");
}

/* ── Saved address ── */
function selectAddr(card) {
  document
    .querySelectorAll(".address-card")
    .forEach((c) => c.classList.remove("active"));
  card.classList.add("active");
}

/* ── Toggle new address ── */
function toggleNewAddr() {
  const f = document.getElementById("newAddrForm");
  f.style.display = f.style.display === "none" ? "block" : "none";
}

/* ── Card formatting ── */
function formatCard(input) {
  let v = input.value.replace(/\D/g, "").slice(0, 16);
  input.value = v.replace(/(.{4})/g, "$1  ").trim();
}
function formatExpiry(input) {
  let v = input.value.replace(/\D/g, "").slice(0, 4);
  if (v.length >= 3) v = v.slice(0, 2) + " / " + v.slice(2);
  input.value = v;
}

/* ── Place order ── */
function placeOrder() {
  const btn = document.querySelector(".btn-place-order");
  btn.textContent = "Đang xử lý...";
  btn.style.opacity = "0.7";
  btn.disabled = true;
  setTimeout(() => {
    document.getElementById("successModal").classList.add("show");
  }, 1600);
}

function closeModal() {
  document.getElementById("successModal").classList.remove("show");
  const btn = document.querySelector(".btn-place-order");
  btn.innerHTML = `<svg width="18" height="18" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24"><rect x="3" y="11" width="18" height="11" rx="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg> Đặt hàng — 4.950.000₫`;
  btn.style.opacity = "1";
  btn.disabled = false;
}

/* Close modal on overlay click */
document.getElementById("successModal").addEventListener("click", function (e) {
  if (e.target === this) closeModal();
});
