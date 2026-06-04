function openTab(btn, id) {
  document
    .querySelectorAll(".tab-btn")
    .forEach((b) => b.classList.remove("active"));
  document
    .querySelectorAll(".tab-content")
    .forEach((c) => c.classList.remove("active"));
  btn.classList.add("active");
  document.getElementById("tab-" + id).classList.add("active");
}
function selectColor(el, name) {
  document
    .querySelectorAll(".color-opt")
    .forEach((c) => c.classList.remove("active"));
  el.classList.add("active");
  document.getElementById("colorName").textContent = name;
}
function selectSize(el) {
  document
    .querySelectorAll(".size-opt")
    .forEach((s) => s.classList.remove("active"));
  el.classList.add("active");
}
function changeQty(delta) {
  const inp = document.getElementById("qty");
  let val = parseInt(inp.value) + delta;
  if (val < 1) val = 1;
  if (val > 10) val = 10;
  inp.value = val;
}
document.querySelectorAll(".thumb").forEach((t) => {
  t.addEventListener("click", () => {
    document
      .querySelectorAll(".thumb")
      .forEach((x) => x.classList.remove("active"));
    t.classList.add("active");
  });
});
