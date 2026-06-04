const prices = { q1: 2890000, q2: 1560000, q3: 890000 };
const qtyMap = { q1: 1, q2: 1, q3: 1 };

function fmt(n) {
  return n.toLocaleString("vi-VN") + "₫";
}

function changeQty(id, delta) {
  qtyMap[id] = Math.max(1, Math.min(10, qtyMap[id] + delta));
  document.getElementById(id).textContent = qtyMap[id];
  const tId = "t" + id.slice(1);
  document.getElementById(tId).textContent = fmt(prices[id] * qtyMap[id]);
  updateSubtotal();
}

function updateSubtotal() {
  let sub = 0;
  for (const k in prices) {
    const item = document.getElementById(k.replace("q", "item"));
    if (item && item.style.display !== "none") sub += prices[k] * qtyMap[k];
  }
  const el = document.getElementById("subtotal");
  if (el) el.textContent = fmt(sub);
}

function removeItem(id) {
  const el = document.getElementById(id);
  if (el) {
    el.style.transition = "opacity 0.3s";
    el.style.opacity = "0";
    setTimeout(() => {
      el.remove();
      updateSubtotal();
    }, 300);
  }
}

document.querySelectorAll(".shipping-option").forEach((opt) => {
  opt.addEventListener("click", () => {
    document
      .querySelectorAll(".shipping-option")
      .forEach((o) => o.classList.remove("active"));
    opt.classList.add("active");
  });
});
