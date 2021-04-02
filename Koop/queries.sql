-- products
select product_name, price, pr.picture, blocked, supplier_abbr, category_name
from products as pr
         left join suppliers s on s.supplier_id = pr.supplier_id
         left join product_categories pc on pr.product_id = pc.product_id
         left join categories c on pc.category_id = c.category_id
order by s.supplier_abbr;

-- cooperator's order
-- order_id corresponds to the week's order
select "FirstName", "LastName", product_name, aq.quantity, unit_name, oi.quantity, order_status_name
from "AspNetUsers" as u
    left join ordered_items oi on u."Id" = oi.coop_id
    left join order_status os on oi.order_status_id = os.order_status_id
    left join products p on oi.product_id = p.product_id
    left join units u2 on p.unit_id = u2.unit_id
    left join available_quantities aq on p.product_id = aq.product_id
where order_id = '00000000-0000-0000-0000-000000000001' and u."Id" = '0fe8d4ce-9e2b-4ca6-9a24-698a8f3e80d4';

-- cooperator
select u."Id", "FirstName", "LastName", "Email", "PhoneNumber", "Name"
from "AspNetUsers" as u
    left join "AspNetUserRoles" ANUR on u."Id" = ANUR."UserId"
    left join "AspNetRoles" ANR on ANUR."RoleId" = ANR."Id"
order by "Name";

-- products
select distinct pc.product_id,
    product_name,
    amount_in_magazine,
    amount_max,
    (select string_agg(ca.category_name, ',') categories
        from categories as ca
            inner join product_categories p on ca.category_id = p.category_id
            where p.product_id = pc.product_id
        group by p.product_id) as categories,
    (select string_agg(cast(quantity as varchar(10)), ',') avail_quantities
        from available_quantities as ava
            where ava.product_id = pc.product_id
        group by ava.product_id) as avail_quantities,
    unit_name,
    cast(price as decimal(10, 2)) as price,
    supplier_abbr,
    available
from products
    inner join units u on products.unit_id = u.unit_id
    inner join product_categories pc on products.product_id = pc.product_id
    inner join categories c on pc.category_id = c.category_id
    inner join available_quantities aq on products.product_id = aq.product_id
    inner join suppliers s on products.supplier_id = s.supplier_id;

-- cooperator's order history
select "FirstName", "LastName", order_stop_date, concat(cast(sum(price) as decimal(10, 2)), ' zl'), order_status_name
from "AspNetUsers" as c
     left join ordered_items oi on c."Id" = oi.coop_id
     left join products pr on oi.product_id = pr.product_id
     left join order_status os on oi.order_status_id = os.order_status_id
     left join orders o on oi.order_id = o.order_id
where c."Id" = '92f36639-f450-445d-9ca1-0cbd710fe301'
group by "FirstName", "LastName", c."Id", order_stop_date, order_status_name;

-- suppliers
select supplier_name, supplier_abbr, s.email, s.phone, order_closing_date, (first_name + ' ' + last_name) as OpRo
from suppliers as s
     inner join cooperators c on s.opro_id = c.coop_id;

-- orders/baskets
select first_name, last_name, basket_id
from cooperators
where basket_id is not null
order by basket_id;

-- paczkers' data
-- select product_name, 



-- create or alter view coop_order_history_view as
-- select c.coop_id, first_name, last_name, order_stop_date, concat(cast(sum(price) as decimal(10, 2)), ' zl') as price, order_status_name
-- from cooperators as c
--          left join ordered_items oi on c.coop_id = oi.coop_id
--          left join products pr on oi.product_id = pr.product_id
--          left join order_status os on oi.order_status_id = os.order_status_id
--          left join orders o on oi.order_id = o.order_id
-- group by first_name, last_name, c.coop_id, order_stop_date, order_status_name;
-- 
-- select * from coop_order_history_view;



-- cooperator's order history view
-- create or replace view user_order_history_view as
--     select "FirstName", "LastName", order_stop_date, concat(cast(sum(price) as decimal(10, 2)), ' zl'), order_status_name
--     from "AspNetUsers" as c
--              left join ordered_items oi on c."Id" = oi.coop_id
--              left join products pr on oi.product_id = pr.product_id
--              left join order_status os on oi.order_status_id = os.order_status_id
--              left join orders o on oi.order_id = o.order_id
--     group by "FirstName", "LastName", c."Id", order_stop_date, order_status_name;


select string_agg(o.order_start_date, ',') pp
from ordered_items as ord_item
    left join orders o on ord_item.order_id = o.order_id
    left join products p on p.product_id = ord_item.product_id
    left join "AspNetUsers" ANU on ord_item.coop_id = ANU."Id"
    left join order_status os on ord_item.order_status_id = os.order_status_id
    left join baskets b on ANU."Id" = b.coop_id
group by o.order_id;


-- cooperator's order history (ewa)
create or replace view user_orders_history_view as
select "Id", "FirstName", "LastName", oi.order_id, order_stop_date, (concat(cast(sum(price) as decimal(10, 2)), ' zl')) as Price, order_status_name
from "AspNetUsers" as u
         left join ordered_items oi on "Id" = coop_id
         left join products pr on pr.product_id = oi.product_id
         left join order_status os on os.order_status_id = oi.order_status_id
         left join orders o on o.order_id = oi.order_id
group by "Id", "FirstName", "LastName", oi.order_id, order_stop_date, order_status_name
order by "Id";

-- supplier (ewa)
create or replace view supplier_view as
select s.supplier_id, supplier_name, supplier_abbr, s.description, s.email, s.phone, s.picture, order_closing_date, s.opro_id, c."FirstName" as opro_first_name, c."LastName" as opro_last_name
from suppliers as s
     inner join "AspNetUsers" c on s.opro_id = c."Id";

-- order grande history (ewa)
drop view order_grande_history_view;
create or replace view order_grande_history_view as
select o.order_id, order_start_date, order_stop_date
from orders as o
         left join order_status os on o.order_status_id = os.order_status_id
;
-- 
-- order basket & name (ewa)
create or replace view baskets_view as
select b.basket_name, concat(u."FirstName", ' ', u."LastName") as Cooperator
from baskets as b
    inner join "AspNetUsers" as u on b.coop_id = u."Id";



select * from "AspNetUsers";